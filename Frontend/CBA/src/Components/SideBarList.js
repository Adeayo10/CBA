import { ROLES } from "../Util/constants";

const listItems = [
  {
    name: "User Management",
    icon: "",
    link: "",
    requiredRoles: [ROLES.SUPER_ADMIN],
    subItems: [
      { name: "Users", icon: "", link: "" },
      { name: "User Roles", icon: "", link: "" },
    ],
  },
];
