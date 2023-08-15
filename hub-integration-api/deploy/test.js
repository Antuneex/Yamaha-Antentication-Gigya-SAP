import http from 'k6/http';
import { Rate } from 'k6/metrics';
import {check} from 'k6';
import crypto from "k6/crypto";
import encoding from "k6/encoding";

const algToHash = {
    HS256: "sha256",
    HS384: "sha384",
    HS512: "sha512"
};

function sign(data, hashAlg, secret) {
    let hasher = crypto.createHMAC(hashAlg, secret);
    hasher.update(data);
    return hasher.digest("base64").replace(/\//g, "_").replace(/\+/g, "-").replace(/=/g, "");
}

function encode(payload, secret, algorithm) {
    algorithm = algorithm || "HS256";
    let header = encoding.b64encode(JSON.stringify({
        typ: "JWT",
        alg: algorithm
    }), "rawurl");
    payload = encoding.b64encode(JSON.stringify(payload), "rawurl");
    let sig = sign(header + "." + payload, algToHash[algorithm], secret);
    return [header, payload, sig].join(".");
}

function decode(token, secret, algorithm) {
    let parts = token.split('.');
    let header = JSON.parse(encoding.b64decode(parts[0], "rawurl"));
    let payload = JSON.parse(encoding.b64decode(parts[1], "rawurl"));
    algorithm = algorithm || algToHash[header.alg];
    if (sign(parts[0] + "." + parts[1], algorithm, secret) != parts[2]) {
        throw Error("JWT signature verification failed");
    }
    return payload;
}

function generateJWTToken(jwtSecret, jwtClient) {
    var dateToExp = new Date(0);
    dateToExp.setUTCSeconds(1234567890);
    let message = {
        "iss": jwtClient,
        "exp": Math.round(dateToExp)
    };
    return encode(message, jwtSecret);
}

const myFailRate = new Rate('failed requests');

export let options = {
  thresholds: {
    'failed requests': ['rate<0.1'], // threshold on a custom metric
    //'http_req_duration': ['p(95)<10000']  // threshold on a standard metric
  }
};

const consumerClientId = '#{TEST_CLIENT_ID}';  
const clientUsername = '#{TEST_CLIENT_USERNAME}';
const serviceName = '#{CONT_NAME}';
const serviceApiRouteUri = '#{SERVICE_API_ROUTE_URI}';
const healthCheckEndpoint = '#{HEALTH_CHECK_ENDPOINT}';

export default function() {
  const res = http.get('http://'+serviceName+healthCheckEndpoint);
  myFailRate.add(res.status != 200);
  check(res, {  'Faz chamada no serviço sem passar pelo kong?': r => r.status == 200 });
  //Check if there is #{TEST_CLIENT_ID} client on kong consumer
  const res_consumer = http.get('http://kong:8001/consumers/'+consumerClientId);
  check(res_consumer, {'O cliente "#{TEST_CLIENT_ID}" existe no Kong?': r => r.json().username == consumerClientId});
  myFailRate.add(res_consumer.status != 200);
  //Check if #{TEST_CLIENT_ID} client has oauth2 credential with client_secret
  const oauth2Keys = http.get('http://kong:8001/consumers/'+consumerClientId+'/oauth2');
  check(oauth2Keys, {'O cliente "#{TEST_CLIENT_ID}" possui chave client_secret oauth2 no Kong?': r => r.json().data[0].client_id == consumerClientId});
  myFailRate.add(oauth2Keys.status != 200);
  const consumerClientSecret = oauth2Keys.json().data[0].client_secret
  //Check if #{TEST_CLIENT_ID} client has jwt credentials
  const jwtKeys = http.get('http://kong:8001/consumers/#{TEST_CLIENT_ID}/jwt');
  check(jwtKeys, {'O cliente "#{TEST_CLIENT_ID}" possui chaves JWT no Kong?': r => r.json().data[0] != null});
  myFailRate.add(jwtKeys.status != 200);
  const jwtBody = jwtKeys.json();
  const jwt_public_key = jwtBody.data[0].key;
  const jwt_secret_key = jwtBody.data[0].secret;

  let bodyToken = {
    grant_type: "password",
    username: clientUsername,
    password: "",
    client_id: consumerClientId,
    client_secret: consumerClientSecret
  }

  let paramsJWTToken = {
    headers: {
      Authorization: "Bearer " + generateJWTToken(jwt_secret_key, jwt_public_key),
    }
  }
  
  //Request oauth2 access_token using jwt token /gambiarra-boa-da-epoca-do-koury /no-password
  const oauth2Token = http.post("http://kong:8000/api/alf/v2/oauth2/token",
    bodyToken,
    paramsJWTToken
  );
  myFailRate.add(oauth2Token.status != 200);
  check(oauth2Token, {  'Consegue gerar access_token via oauth2 sem senha (usando JWT) pelo Kong?': r => r.status == 200 });

  
  //Check if service request works with authentication
  const accessTokenHeaders = {
      'Authorization': 'Bearer ' + oauth2Token.json().access_token,
    }

  const res_with_kong = http.get('http://kong:8000'+serviceApiRouteUri+healthCheckEndpoint, {headers: accessTokenHeaders});
  myFailRate.add(res_with_kong.status != 200);
  check(res_with_kong, {  'Faz chamada com token oauth2 no serviço passando pelo Kong?': r => r.status == 200 });

}