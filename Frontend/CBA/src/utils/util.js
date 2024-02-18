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

export function generateUserId() {
  return crypto.randomUUID();
}

export function extractUpdateFields(dataObject, targetObject, excluded=[]){
  const output = {};

  for (const key of Object.keys(targetObject)) {
    if (excluded.includes(key)) continue;
    output[key] = dataObject[key];
  }

  return output
}
