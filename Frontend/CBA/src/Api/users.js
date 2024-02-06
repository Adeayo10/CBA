export async function getUsers(limit=0){
  const API_URL = "/api/v1/Auth/GetAllUsers";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(limit);
  const response = await fetch(API_URL, {
    method: "GET",
    headers,
  });
  return await response.json();
}

export async function getUserById(id){
  const API_URL = `/api/v1/Auth/GetUser?id=${id}`;
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(id)
  const response = await fetch(API_URL, {
    method: "GET",
    headers,
  });
  return await response.json();
}