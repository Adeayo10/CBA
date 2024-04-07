import { refreshAccessToken, getAuthorizationHeader } from "./auth";

import { tokenExpired } from "../utils/token";

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
  const API_URL = `/api/v1/Customer/GetCustomer?id=${id}`;

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

export async function getCustomerAccountTypes() {
  const API_URL = `/api/v1/Customer/AccountTypes`;

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

export async function changeCustomerAccountStatus(customerId) {
  const API_URL = "/api/v1/Customer/ChangeCustomerAccountStatus";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(customerId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify({ customerId }),
  });
  return await response.json();
}

export async function getCustomerTransactions(accountStamentDetails) {
  const API_URL = "/api/v1/Customer/GetCustomerTransactions";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  //console.log(userId);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(accountStamentDetails),
  });
  return await response.json();
}

export async function generateAccountStatement(accountStamentDetails) {
  const API_URL = "/api/v1/Customer/CreateAccountStatementPdf";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(accountStamentDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(accountStamentDetails),
  });
  return await response.json();
}
