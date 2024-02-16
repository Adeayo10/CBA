import Cookies from "universal-cookie";

const cookies = new Cookies();

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

export async function logoutUser() {
  const API_URL = "/api/v1/Token/RevokeToken";

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
  console.log(email);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ email }),
  });
  return await response.json();
}

export async function refreshAccessToken() {
  const API_URL = "/api/v1/Token/RefreshToken";
  const headers = {
    "Content-Type": "application/json",
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
  const { token, refreshToken, expiryDate } = await response.json();
  saveTokenData(token, refreshToken, expiryDate);
}

export function getCurrentRole() {
  //todo implement role fetching logic
  return "SuperAdmin";
}

export function getAuthorizationHeader() {
  return `Bearer ${retrieveAccessToken()}`;
}

export function saveTokenData(accessToken, refreshToken, expiryDate) {
  cookies.set("accessToken", accessToken, {
    path: "/",
    secure: true,
    sameSite: false,
  });

  cookies.set("refreshToken", refreshToken, {
    path: "/",
    secure: true,
    sameSite: false,
  });

  cookies.set("expiryDate", expiryDate, {
    path: "/",
    secure: true,
    sameSite: false,
  });
}

export function clearTokenData() {
  cookies.remove("accessToken", { path: "/" });
  cookies.remove("refreshToken", { path: "/" });
  cookies.remove("expiryDate", { path: "/" });
}

export function tokenExists() {
  return Boolean(retrieveAccessToken());
}

export function tokenExpired() {
  const expiryDate = retrieveExpiryDate();
  return new Date(expiryDate).getTime() > new Date().getTime();
}

function retrieveAccessToken() {
  return cookies.get("accessToken");
}

function retrieveRefreshToken() {
  return cookies.get("refreshToken");
}

function retrieveExpiryDate() {
  return cookies.get("expiryDate");
}
