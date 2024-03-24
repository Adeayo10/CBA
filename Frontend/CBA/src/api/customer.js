import { refreshAccessToken, getAuthorizationHeader } from "./auth";

import { tokenExpired } from "../utils/token";

import { PAGE_SIZE } from "../utils/constants";

export async function getCustomers(
  accountType,
  pageNumber = 1,
  pageSize = PAGE_SIZE
) {
  const API_URL = `/api/v1/Customer/GetAllCustomers?pageNumber=${pageNumber}&pageSize=${pageSize}&filterValue=${accountType}`;

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

export async function getCustomerById(id) {
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

export async function createCustomer(customerDetails) {
  const API_URL = "/api/v1/Customer/CreateCustomer";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(customerDetails);
  const response = await fetch(API_URL, {
    method: "POST",
    headers,
    body: JSON.stringify(customerDetails),
  });
  return await response.json();
}

export async function updateCustomer(customerDetails) {
  const API_URL = "/api/v1/Customer/UpdateCustomer";

  if (tokenExpired()) await refreshAccessToken();

  const headers = {
    "Content-Type": "application/json",
    Authorization: getAuthorizationHeader(),
  };
  console.log(customerDetails);
  const response = await fetch(API_URL, {
    method: "PUT",
    headers,
    body: JSON.stringify(customerDetails),
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

export async function getCustomerTransactions(userId) {
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
    body: JSON.stringify({ userId }),
  });
  return await response.json();
}
