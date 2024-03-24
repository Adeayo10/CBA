export const TOAST_CONFIG = {
  position: "top-right",
  autoClose: 5000,
  hideProgressBar: false,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
  progress: undefined,
};

export const DRAWER_WIDTH = 280;

export const ROLES = {
  SUPER_ADMIN: "SuperAdmin",
  ADMIN: "Admin",
  MANAGER: "Manager",
  USER: "User",
};

export const STATUS = {
  ACTIVE: "Active",
  DEACTIVATED: "Deactivated",
};

export const USER_ALLLOWED_FIELDS = [
  "fullName",
  "userName",
  "email",
  "email Confirmed",
  "address",
  "status",
  "role",
  "phone Number",
  "phone Number Confirmed",
  "two Factor Enabled",
];

export const USER_BRANCH_ALLOWED_FIELDS = [
  "name",
  "region",
  "code",
  "description",
];

export const CREATE_USER_BASE = {
  id: "",
  userName: "",
  email: "",
  password: "",
  fullName: "",
  address: "",
  phoneNumber: "",
  status: "Active",
  role: ROLES.USER,
};

export const CREATE_USER_BRANCH_BASE = {
  userId: "",
  name: "",
  region: "",
  code: "default",
  description: "",
};

export const ROUTES = {
  BASE_PATH: "/",
  ACCOUNT_STATEMENT: "account-statement",
  CURRENT_ACCOUNTS: "current-accounts",
  GENERAL_LEDGER: "general-ledger",
  CUSTOMER_ACCOUNTS: "customer-accounts",
  CUSTOMER_INFO: "customer-information",
  LOANS: "loan-accounts",
  PROFILE: "profile",
  SAVINGS_ACCOUNTS: "savings-accounts",
  USER_ROLES: "user-roles",
  USERS: "users",
  DASHBOARD: "/dashboard",
  FORGOT_PASSWORD: "/forgot-password",
  LOGIN: "/login",
  RESET_PASSWORD: "/reset-password",
  VERIFY_TOKEN: "/verify-token",
};

export const ACCOUNT_TYPES = {
  SAVINGS: "Savings",
  LOAN: "Loan",
  CURRENT: "Current",
};

export const PAGE_SIZE = 10;

export const GENDER = {
  MALE: "Male",
  FEMALE: "Female"
}

export const CREATE_ACCOUNT_BASE = {
  id: "",
  fullName: "",
  email: "",
  phoneNumber: "",
  address: "",
  gender: GENDER.MALE,
  branch: "",
  accountNumber: "",
  status: STATUS.ACTIVE,
  accountType: "",
  state: STATUS.ACTIVE
}