import { APP_LOCALE } from "./constants";

import dayjs from "dayjs";

export function capitalize(string) {
  if (!string) return "";
  return string
    .split(" ")
    .map((word) =>
      word.length > 0 ? word[0].toUpperCase() + word.slice(1).toLowerCase() : ""
    )
    .join(" ");
}

export function generateRandomPassword() {
  return (
    Math.random().toString(36).slice(2) +
    "." +
    Math.random().toString(36).toUpperCase().slice(2)
  );
}

export function generateId() {
  return crypto.randomUUID();
}

export function generateAccountNumber() {
  const min = 1;
  const max = 100000000000;
  const randNum = Math.floor(Math.random() * (max - min)) + min;
  return randNum.toString().padStart(10, "0");
}

export function extractUpdateFields(dataObject, targetObject, excluded = []) {
  const output = {};

  for (const key of Object.keys(targetObject)) {
    if (excluded.includes(key)) continue;
    output[key] = dataObject[key];
  }

  return output;
}

export function formatDate(dateString) {
  return dayjs(dateString).format("YYYY-MM-DD");
}

export function getCurrentISODate() {
  return dayjs().toISOString();
}

export function formatCurrency(amount) {
  return new Intl.NumberFormat(APP_LOCALE.REGION, {
    style: "currency",
    currency: APP_LOCALE.CURRENCY,
  }).format(amount);
}
