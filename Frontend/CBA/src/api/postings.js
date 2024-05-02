import { refreshAccessToken, getAuthorizationHeader } from "./auth";

import { tokenExpired } from "../utils/token";

import { PAGE_SIZE, ACCOUNT_IDS } from "../utils/constants";

export async function createTransfer(transferPosting) {
  const API_URL = `/api/v1/Posting/Transfer`;

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(transferPosting),
  });
  return await response.json();
}

export async function createWithdrawal(withdrawalPosting) {
  const API_URL = `/api/v1/Posting/Withdraw`;

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(withdrawalPosting),
  });
  console.log(response)
  return await response.json();
}

export async function createDeposit(depositPosting) {
  const API_URL = `/api/v1/Posting/Deposit`;

  console.log({ depositPosting });
  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(depositPosting),
  });
  return await response.json();
}

export async function getPostings(
  postingType,
  pageNumber = 1,
  pageSize = PAGE_SIZE
) {
  const API_URL = `/api/v1/Posting/GetPostings?pageNumber=${pageNumber}&pageSize=${pageSize}&filterValue=${postingType}`;

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
