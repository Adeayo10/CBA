import { refreshAccessToken, getAuthorizationHeader } from "./auth";

import { tokenExpired } from "../utils/token";

export async function getUsers(limit = 0) {
  const API_URL = "/api/v1/Auth/GetAllUsers";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(limit);
  const response = await fetch(API_URL, {
    method: "GET",
    headers,
  });
  return await response.json();
}

export async function getUserById(id) {
  const API_URL = `/api/v1/Auth/GetUser?id=${id}`;

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(id);
  const response = await fetch(API_URL, {
    method: "GET",
    headers,
  });
  return await response.json();
}

export async function getUserRoles() {
  const API_URL = `/api/v1/Auth/get-roles`;

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(id);
  const response = await fetch(API_URL, {
    method: "GET",
    headers,
  });
  return await response.json();
}

export async function createUser(userDetails) {
  const API_URL = "/api/v1/Auth/Register";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(userDetails),
  });
  return await response.json();
}

export async function updateUser(userDetails) {
  const API_URL = "/api/v1/Auth/UpdateUser";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userDetails);
  const response = await fetch(API_URL, {
    method: "PUT",
    headers,
    body: JSON.stringify(userDetails),
  });
  return await response.json();
}

export async function deactivateUser(userId){
  const API_URL = "/api/v1/Auth/deactivate-user";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({userId}),
  });
  return await response.json();
}

export async function activateUser(userId){
  const API_URL = "/api/v1/Auth/activate-user";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({userId}),
  });
  return await response.json();
}