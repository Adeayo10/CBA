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
  USER: "User",
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

export const USER_BRANCH_ALLOWED_FIELDS = ["name", "region", "code", "description"];

export const CREATE_USER_BASE = {
  id: "",
  userName: "",
  email: "",
  password: "",
  fullName: "",
  address: "",
  phoneNumber: "",
  status: "",
  role: ROLES.USER,
};

export const CREATE_USER_BRANCH_BASE = {
  userId: "",
  name: "",
  region: "",
  code: "",
  description: "",
};
