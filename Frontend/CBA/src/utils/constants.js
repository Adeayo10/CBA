import { generateId } from "./util";

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
  CREATE_POSTING: "create-posting",
  DEPOSITS: "deposits",
  WITHDRAWALS: "withdrawals",
  TRANSFERS: "transfers",
  DASHBOARD: "/dashboard",
  FORGOT_PASSWORD: "/forgot-password",
  LOGIN: "/login",
  RESET_PASSWORD: "/reset-password",
  VERIFY_TOKEN: "/verify-token",
  NULL: "#",
  NOT_FOUND: "/404",
};

export const ACCOUNT_TYPES = {
  SAVINGS: "Savings",
  LOAN: "Loan",
  CURRENT: "Current",
};

export const POSTING_TYPES = {
  DEPOSIT: "Deposit",
  WITHDRAWAL: "Withdrawal",
  TRANSFER: "Transfer",
};

export const ACCOUNT_IDS = { Current: 2, Savings: 1, Loan: 3 };

export const PAGE_SIZE = 10;

export const GENDER = {
  MALE: "Male",
  FEMALE: "Female",
};

export const NG_STATES = [
  "Abia",
  "Adamawa",
  "Akwa Ibom",
  "Anambra",
  "Bauchi",
  "Bayelsa",
  "Benue",
  "Borno",
  "Cross River",
  "Delta",
  "Ebonyi",
  "Edo",
  "Ekiti",
  "Enugu",
  "FCT - Abuja",
  "Gombe",
  "Imo",
  "Jigawa",
  "Kaduna",
  "Kano",
  "Katsina",
  "Kebbi",
  "Kogi",
  "Kwara",
  "Lagos",
  "Nasarawa",
  "Niger",
  "Ogun",
  "Ondo",
  "Osun",
  "Oyo",
  "Plateau",
  "Rivers",
  "Sokoto",
  "Taraba",
  "Yobe",
  "Zamfara"
]

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
  state: NG_STATES[0],
};

export const ACCOUNT_ALLLOWED_FIELDS = [
  "fullName",
  "email",
  "gender",
  "phone Number",
  "address",
  "account Number",
  "account Type",
  "status",
  "balance",
  "date Created",
];

export const CREATE_POSTING_BASE = {
  [POSTING_TYPES.DEPOSIT]: {
    id: "",
    ledgerAccountName: "",
    ledgerAccountNumber: "",
    amount: "",
    transactionType: POSTING_TYPES.DEPOSIT,
    narration: "",
    datePosted: "",
    customerAccountNumber: "",
    customerAccountName: "",
    customerNarration: "",
    customerTransactionType: POSTING_TYPES.DEPOSIT,
  },
  [POSTING_TYPES.WITHDRAWAL]: {
    id: "",
    ledgerAccountName: "",
    ledgerAccountNumber: "",
    amount: "",
    transactionType: POSTING_TYPES.WITHDRAWAL,
    narration: "",
    datePosted: "",
    customerAccountNumber: "",
    customerAccountName: "",
    customerNarration: "",
    customerTransactionType: POSTING_TYPES.WITHDRAWAL,
  },
  [POSTING_TYPES.TRANSFER]: {
    senderId: generateId(),
    receiverId: generateId(),
    amount: "",
    narration: "",
    senderAccountNumber: "",
    receiverAccountNumber: "",
    receiverAccountName: "",
  },
};

export const POSTING_AUTOGEN_FIELD_MAP = {
  status: "status",
  customerId: "id",
  customerName: "fullName",
  customerAccountType: "accountType",
  customerBranch: "branch",
  customerEmail: "email",
  customerPhoneNumber: "phoneNumber",
  customerStatus: "status",
  customerGender: "gender",
  customerAddress: "address",
  customerState: "state",
  customerDateCreated: "dateCreated",
  customerBalance: "balance",
  customerFullName: "fullName",
};

export const LEDGER_TYPES = {
  ASSET: "Asset",
  LIABILITY: "Liability",
  CAPITAL: "Capital",
  INCOME: "Income",
  EXPENSE: "Expense",
};

export const CREATE_LEDGER_BASE = {
  accountName: "",
  accountDescription: "",
  accountCategory: LEDGER_TYPES.ASSET,
};

export const LEDGER_ALLLOWED_FIELDS = [
  "accountName",
  "accountDescription",
  "accountCategory",
  "balance",
];

export const APP_LOCALE = { REGION: "en-NG", CURRENCY: "NGN" };


