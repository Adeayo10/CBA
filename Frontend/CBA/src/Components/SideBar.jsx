import { useState } from "react";
import { styled } from "@mui/material/styles";
import MuiDrawer from "@mui/material/Drawer";
import Toolbar from "@mui/material/Toolbar";
import List from "@mui/material/List";
import Divider from "@mui/material/Divider";
import IconButton from "@mui/material/IconButton";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import Collapse from "@mui/material/Collapse";
import Avatar from "@mui/material/Avatar";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Person2Icon from "@mui/icons-material/Person2";
import InboxIcon from "@mui/icons-material/MoveToInbox";
import DraftsIcon from "@mui/icons-material/Drafts";
import SendIcon from "@mui/icons-material/Send";
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import StarBorder from "@mui/icons-material/StarBorder";
import PeopleIcon from "@mui/icons-material/People";
import ManageAccountsIcon from "@mui/icons-material/ManageAccounts";
import AdminPanelSettingsIcon from "@mui/icons-material/AdminPanelSettings";
import HomeIcon from "@mui/icons-material/Home";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import ListSubheader from "@mui/material/ListSubheader";
import { Link as RouterLink, useNavigate } from "react-router-dom";

import { DRAWER_WIDTH, ROLES, ROUTES } from "../utils/constants";
import { retrieveUserFromToken } from "../utils/token";
import { capitalize } from "../utils/util";

const Drawer = styled(MuiDrawer, {
  shouldForwardProp: (prop) => prop !== "open",
})(({ theme, open }) => ({
  "& .MuiDrawer-paper": {
    position: "relative",
    whiteSpace: "nowrap",
    width: DRAWER_WIDTH,
    transition: theme.transitions.create("width", {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
    boxSizing: "border-box",
    ...(!open && {
      overflowX: "hidden",
      transition: theme.transitions.create("width", {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
      }),
      width: theme.spacing(7),
      [theme.breakpoints.up("sm")]: {
        width: theme.spacing(9),
      },
    }),
  },
}));

const listItems = [
  {
    name: "Home",
    icon: <HomeIcon />,
    linkTo: ".",
    requiredRoles: [],
    subItems: [],
  },
  {
    name: "User Management",
    icon: <AdminPanelSettingsIcon />,
    linkTo: "#",
    requiredRoles: [ROLES.SUPER_ADMIN],
    subItems: [
      { name: "Users", icon: <PeopleIcon />, linkTo: ROUTES.USERS },
      {
        name: "User Roles",
        icon: <ManageAccountsIcon />,
        linkTo: ROUTES.USER_ROLES,
      },
    ],
  },
  {
    name: "Account Management",
    icon: <AdminPanelSettingsIcon />,
    linkTo: "#",
    requiredRoles: [ROLES.SUPER_ADMIN],
    subItems: [
      {
        name: "Customer Account",
        icon: <PeopleIcon />,
        linkTo: ROUTES.CUSTOMER_ACCOUNTS,
      },
      {
        name: "Account Statement",
        icon: <ManageAccountsIcon />,
        linkTo: ROUTES.ACCOUNT_STATEMENT,
      },
      {
        name: "Current Accounts",
        icon: <ManageAccountsIcon />,
        linkTo: ROUTES.CURRENT_ACCOUNTS,
      },
      {
        name: "Savings Accounts",
        icon: <ManageAccountsIcon />,
        linkTo: ROUTES.SAVINGS_ACCOUNTS,
      },
      // {
      //   name: "Loans",
      //   icon: <ManageAccountsIcon />,
      //   linkTo: ROUTES.LOANS,
      // },
    ],
  },
  {
    name: "Customer Information",
    icon: <HomeIcon />,
    linkTo: ROUTES.CUSTOMER_INFO,
    requiredRoles: [],
    subItems: [],
  },
  {
    name: "General Ledger",
    icon: <HomeIcon />,
    linkTo: ROUTES.GENERAL_LEDGER,
    requiredRoles: [],
    subItems: [],
  },
];

function ListElement({
  name,
  icon,
  linkTo = ".",
  requiredRoles,
  subItems = [],
  sideBarOpen,
}) {
  const hasSubItems = subItems.length > 0;
  const [subItemsOpen, setSubItemsOpen] = useState(true);
  const itemKey = `${name.replace(" ", "_")}`;

  const toggleSubItems = () => {
    setSubItemsOpen(!subItemsOpen);
  };
  return (
    <>
      <RouterLink to={linkTo} key={`${itemKey}_link`}>
        <ListItemButton onClick={toggleSubItems} key={itemKey}>
          <ListItemIcon>{icon}</ListItemIcon>
          <ListItemText primary={name} />
          {hasSubItems && sideBarOpen ? (
            subItemsOpen ? (
              <ExpandLess />
            ) : (
              <ExpandMore />
            )
          ) : (
            ""
          )}
        </ListItemButton>
      </RouterLink>

      <Collapse
        in={hasSubItems && subItemsOpen}
        timeout="auto"
        unmountOnExit
        key={`${itemKey}_collapse`}
      >
        <List component="div" disablePadding>
          {subItems.map(({ name, icon, linkTo = "." }, index) => {
            const subItemKey = `${name.replace(" ", "_")}_${index}`;
            return (
              <RouterLink to={linkTo} key={`${subItemKey}_link`}>
                <ListItemButton sx={{ pl: 4 }} key={subItemKey}>
                  <ListItemIcon>{icon}</ListItemIcon>
                  <ListItemText primary={name} />
                </ListItemButton>
              </RouterLink>
            );
          })}
        </List>
      </Collapse>
    </>
  );
}

// function ListElementNew({
//   name,
//   icon,
//   linkTo = ".",
//   requiredRoles,
//   subItems = [],
//   sideBarOpen,
// }) {
//   const hasSubItems = subItems.length > 0;
//   const [subItemsOpen, setSubItemsOpen] = useState(true);
//   const itemKey = `${name.replace(" ", "_")}`;

//   const toggleSubItems = () => {
//     setSubItemsOpen(!subItemsOpen);
//   };

//   if (hasSubItems)
//     return (
//       <>
//         <ListSubheader component="div" id="nested-list-subheader">
//           {icon} {name}
//         </ListSubheader>
//         {subItems.map(({ name, icon, linkTo = "." }, index) => {
//           const subItemKey = `${name.replace(" ", "_")}_${index}`;
//           return (
//             <RouterLink to={linkTo} key={`${subItemKey}_link`}>
//               <ListItemButton sx={{ pl: 4 }} key={subItemKey}>
//                 <ListItemIcon>{icon}</ListItemIcon>
//                 <ListItemText primary={name} />
//               </ListItemButton>
//             </RouterLink>
//           );
//         })}
//       </>
//     );
//   return (
//     <>
//       <RouterLink to={linkTo} key={`${itemKey}_link`}>
//         <ListItemButton key={itemKey}>
//           <ListItemIcon>{icon}</ListItemIcon>
//           <ListItemText primary={name} />
//         </ListItemButton>
//       </RouterLink>
//     </>
//   );
// }

export default function SideBar({ sideBarOpen, toggleSideBar }) {
  const [loggedInUser, setLoggedInUser] = useState(retrieveUserFromToken());
  return (
    <Drawer variant="permanent" open={sideBarOpen}>
      <Toolbar
        sx={{
          display: "flex",
          alignItems: "center",
          justifyContent: "flex-end",
          px: [1],
        }}
      >
        <RouterLink to={"/dashboard/profile"} style={{ flex: 1 }}>
          <Box
            sx={{
              display: "flex",
              marginRight: "0px",
              marginLeft: "0px",
              alignItems: "center",

              ...(!sideBarOpen && { display: "none" }),
            }}
          >
            <Avatar
              sx={{
                m: 1,
                bgcolor: "secondary.main",
              }}
            >
              <Person2Icon />
            </Avatar>
            <Typography
              variant="h6"
              gutterBottom
              sx={{
                m: 1,
              }}
            >
              {capitalize(loggedInUser?.username)}
            </Typography>
          </Box>
        </RouterLink>
        <IconButton onClick={toggleSideBar}>
          <ChevronLeftIcon />
        </IconButton>
      </Toolbar>
      <Divider sx={{ mb: 1, width: "100%" }} />
      <List component="nav" disablePadding>
        {listItems.map(
          (
            { name, icon, linkTo = ".", requiredRoles, subItems = [] },
            index
          ) => {
            return (
              <ListElement
                name={name}
                icon={icon}
                linkTo={linkTo}
                requiredRoles={requiredRoles}
                subItems={subItems}
                sideBarOpen={sideBarOpen}
                key={`${name.replace(" ", "_")}_${index}`}
              />
            );
          }
        )}
      </List>
    </Drawer>
  );
}
