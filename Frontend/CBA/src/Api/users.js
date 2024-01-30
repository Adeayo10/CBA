export function getUsers(limit=0){
  const API_URL = "/api/v1/Auth/GetAllUsers";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(limit);
  return fetch(API_URL, {
    method: "GET",
    headers,
  }).then((response) => response.json());
}

export function getUserById(id){
  const API_URL = `/api/v1/Auth/GetUser?id=${id}`;
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(id)
  return fetch(API_URL, {
    method: "GET",
    headers,
  }).then((response) => response.json());
}