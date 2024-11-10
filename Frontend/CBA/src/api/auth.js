import {
  retrieveRefreshToken,
  retrieveAccessToken,
  tokenExpired,
  saveTokenData,
  retrieveUserId,
} from "../utils/token";

export async function loginUser(loginDetails) {
  const API_URL = "/api/v1/Auth/Login";
  const headers = {
    "Content-Type": "application/json",
  };
  //console.log(loginDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(loginDetails),
  });
  return await response.json();
}

export async function logoutUser() {
  const API_URL = "/api/v1/Auth/Logout";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };

  return await fetch(API_URL, {
    method: "POST",
    headers,
  });
  // console.log(response)
  // return await response.json();
}

export async function forgotPassword(email) {
  const API_URL = "/api/v1/Auth/forget-password";
  const headers = {
    "Content-Type": "application/json",
  };
  //console.log(email);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ email }),
  });
  return await response.json();
}

export async function verifyCode(code, userId) {
  userId = userId || retrieveUserId()
  const API_URL = `/api/v1/Auth/verify-token?token=${code}`;
  const headers = {
    "Content-Type": "application/json",
  };
  //console.log(loginDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ userId }),
  });
  return await response.json();
}

export async function resendCode() {
  const userId = retrieveUserId()
  const API_URL = `/api/v1/Auth/resend-token`;
  const headers = {
    "Content-Type": "application/json",
  };
  //console.log(loginDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ userId }),
  });
  return await response.json();
}

export async function resetPassword(requestBody) {
  const API_URL = "/api/v1/Auth/reset-password";
  const headers = {
    "Content-Type": "application/json",
  };
  //console.log(requestBody);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(requestBody),
  });
  return await response.json();
}

export async function changePassword(requestBody) {
  const API_URL = "/api/v1/Auth/change-password";

  if (tokenExpired()) refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(requestBody);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(requestBody),
  });
  return await response.json();
}

export async function refreshAccessToken() {
  const API_URL = "/api/v1/Token/RefreshToken";
  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  const requestBody = {
    token: retrieveAccessToken(),
    refreshToken: retrieveRefreshToken(),
  };
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(requestBody),
  });
  if (response.status == 401)
    throw new Error("Refresh Token Expired")
  const { token, refreshToken, expiryDate, errors } = await response.json();

  if (errors) throw new Error(errors);

  saveTokenData(token, refreshToken, expiryDate);
  //console.log("Refreshed token");
}

export function getAuthorizationHeader() {
  return `Bearer ${retrieveAccessToken()}`;
}
