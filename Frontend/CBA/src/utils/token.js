import Cookies from "universal-cookie";
import { jwtDecode } from "jwt-decode";
import { ROUTES } from "./constants";

const cookies = new Cookies();

export function getCurrentRole() {
  //todo implement role fetching logic
  return "SuperAdmin";
}

export function saveUserId(userId) {
  cookies.set("userId", userId, {
    path: "/",
    secure: true,
    sameSite: false,
  });
}

export function retrieveUserId() {
  return cookies.get("userId");
}

export function saveTokenData(accessToken, refreshToken, expiryDate) {
  cookies.set("clientAccessToken", accessToken, {
    path: "/",
    secure: true,
    sameSite: false,
  });

  cookies.set("clientRefreshToken", refreshToken, {
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
  cookies.remove("clientAccessToken", { path: "/" });
  cookies.remove("clientRefreshToken", { path: "/" });
  cookies.remove("expiryDate", { path: "/" });
  cookies.remove("userId", { path: "/" });
}

export function tokenExists() {
  console.log({accessToken: retrieveAccessToken()})
  return Boolean(retrieveAccessToken());
}

export function tokenExpired() {
  const expiryDate = retrieveExpiryDate();
  console.log(expiryDate)
  if (!expiryDate) return true
  const timeOffsetMs = 30000;
  return new Date().getTime() > new Date(expiryDate).getTime() - timeOffsetMs;
}

export function redirectIfRefreshTokenExpired(errorMessage, navigate) {
  if (!errorMessage.toLowerCase().includes("expired")) return;
  clearTokenData();
  navigate(ROUTES.LOGIN);
}

export function retrieveRefreshToken() {
  return cookies.get("clientRefreshToken");
}

export function retrieveAccessToken() {
  return cookies.get("clientAccessToken");
}

export function retrieveUserFromToken() {
  const { role, email } = jwtDecode(retrieveAccessToken());
  return { role, email};
}

function retrieveExpiryDate() {
  return cookies.get("expiryDate");
}
