export async function loginUser(loginDetails) {
  const API_URL = "/api/v1/Auth/Login";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(loginDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(loginDetails),
  });
  return await response.json();
}

export async function refreshToken(){
  const API_URL = "/api/v1/Auth/Login";
  const headers = {
    "Content-Type": "application/json",
  };
  console.log(loginDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(loginDetails),
  });
  return await response.json();
}