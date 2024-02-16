export function isValidPhoneNumber(phoneNumber) {
  const phoneStartsWithCorrectSymbol =
    phoneNumber.startsWith("0") || phoneNumber.startsWith("+");
  const phoneNumberHasCorrectLength =
    phoneNumber.slice(1).length > 9 && phoneNumber.slice(1).length < 14;
  const phoneNumberIsNumber = !isNaN(Number(phoneNumber.slice(1)));

  console.log(phoneNumberHasCorrectLength);
  return (
    phoneStartsWithCorrectSymbol &&
    phoneNumberHasCorrectLength &&
    phoneNumberIsNumber
  );
}

export function isValidEmail(email) {
  return String(email)
    .toLowerCase()
    .match(
      /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    );
}
