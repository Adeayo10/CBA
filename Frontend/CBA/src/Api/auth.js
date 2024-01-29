export function loginUser(loginDetails) {
  const API_URL = "/api/v1/Auth/Login";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(loginDetails);
  return fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(loginDetails),
  }).then((response) => response.json());
}

export function refreshToken(){
  const API_URL = "/api/v1/Auth/Login";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(loginDetails);
  return fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(loginDetails),
  }).then((response) => response.json());
}