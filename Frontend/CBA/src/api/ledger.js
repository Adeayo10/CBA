import { refreshAccessToken, getAuthorizationHeader } from "./auth";

import { retrieveUserId, tokenExpired } from "../utils/token";

import { PAGE_SIZE, ACCOUNT_IDS } from "../utils/constants";

export async function getLedgers(pageNumber = 1, pageSize = PAGE_SIZE) {
  const API_URL = `/api/v1/Ledger/getGLAccounts?pageNumber=${pageNumber}&pageSize=${pageSize}`;

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

export async function getLedgerById(id) {
  const API_URL = `/api/v1/Ledger/getGLAccountById?id=${id}`;

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

export async function getLedgerBalance(accountNumber) {
  const API_URL = `/api/v1/Ledger/viewLedgerAccountBalance?accountNumber=${accountNumber}`;

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

export async function createLedger(ledgerDetails) {
  const API_URL = "/api/v1/Ledger/createGLAccount";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(ledgerDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(ledgerDetails),
  });
  return await response.json();
}

export async function updateLedger(ledgerDetails) {
  const API_URL = "/api/v1/Ledger/updateGLAccount";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(ledgerDetails);
  const response = await fetch(API_URL, {
    method: "PUT",
    headers,
    body: JSON.stringify(ledgerDetails),
  });
  return await response.json();
}

export async function changeLedgerStatus(ledgerId) {
  const API_URL = `/api/v1/Ledger/changeAccountStatus?id=${ledgerId}`;

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(ledgerId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    // body: JSON.stringify({ id: ledgerId }),
  });
  return await response;
}

export async function linkUserToLedger(ledgerId) {
  const API_URL = "/api/v1/Ledger/linkUserToGLAccount";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ glAccountid: ledgerId, userid: retrieveUserId() }),
  });
  return await response.json();
}

export async function unlinkUserFromLedger(ledgerId) {
  const API_URL = "/api/v1/Ledger/unLinkUserToGLAccount";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ glAccountid: ledgerId, userid: retrieveUserId() }),
  });
  return await response.json();
}
