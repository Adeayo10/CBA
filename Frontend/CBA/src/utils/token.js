import Cookies from "universal-cookie";
import { jwtDecode } from "jwt-decode";
import { ROUTES} from "./constants";

const cookies = new Cookies();

export function getCurrentRole() {
  //todo implement role fetching logic
  return "SuperAdmin";
}

export function saveUserId(userId){
  cookies.set("userId", userId, {
    path: "/",
    secure: true,
    sameSite: false,
  });
}

export function retrieveUserId(){
  return cookies.get("userId")
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
  cookies.remove("userId", {path: "/"})
}

export function tokenExists() {
  return Boolean(retrieveAccessToken());
}

export function tokenExpired() {
  const expiryDate = retrieveExpiryDate();
  return new Date().getTime() > new Date(expiryDate).getTime();
}

export function redirectIfRefreshTokenExpired(errorMessage, navigate) {
  if (!errorMessage.toLowerCase().includes("expired")) return;
  clearTokenData();
  navigate(ROUTES.LOGIN);
}

export function retrieveRefreshToken() {
  return cookies.get("refreshToken");
}

export function retrieveAccessToken() {
  return cookies.get("accessToken");
}

export function retrieveUserFromToken() {
  const { role, email, unique_name } = jwtDecode(retrieveAccessToken());
  return { role, email, username: unique_name };
}

function retrieveExpiryDate() {
  return cookies.get("expiryDate");
}
