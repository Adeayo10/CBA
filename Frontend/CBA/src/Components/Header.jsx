import { useState } from "react";
import { styled } from "@mui/material/styles";
import MuiAppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import IconButton from "@mui/material/IconButton";
import Badge from "@mui/material/Badge";
import MenuIcon from "@mui/icons-material/Menu";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import NotificationsIcon from "@mui/icons-material/Notifications";
import LogoutIcon from "@mui/icons-material/Logout";
import Button from "@mui/material/Button";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";
import Avatar from "@mui/material/Avatar";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Person2Icon from '@mui/icons-material/Person2';

import { Link as RouterLink, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import { DRAWER_WIDTH, TOAST_CONFIG, ROUTES } from "../utils/constants";
import { clearTokenData } from "../utils/token";
import { logoutUser } from "../api/auth";

import LogoutModal from "./LogoutModal";

const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== "open",
})(({ theme, open }) => ({
  zIndex: theme.zIndex.drawer + 1,
  transition: theme.transitions.create(["width", "margin"], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  ...(open && {
    marginLeft: DRAWER_WIDTH,
    width: `calc(100% - ${DRAWER_WIDTH}px)`,
    transition: theme.transitions.create(["width", "margin"], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

export default function Header({ sideBarOpen, toggleSideBar }) {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [modalOpen, setModalOpen] = useState(false)

  const openModal = () =>{
    setModalOpen(true)
  }

  const closeModal = () =>{
    setModalOpen(false)
  }

  return (
    <AppBar position="absolute" open={sideBarOpen}>
      <Toolbar
        sx={{
          pr: "24px", // keep right padding when drawer closed
        }}
      >
        <RouterLink to={ROUTES.PROFILE}>
          <Avatar
            sx={{
              m: 1,
              bgcolor: "secondary.main",
              marginRight: "0px",
              marginLeft: "0px",
              ...(sideBarOpen && { display: "none" }),
            }}
          >
            <Person2Icon />
          </Avatar>
        </RouterLink>
        <IconButton
          edge="start"
          color="inherit"
          aria-label="open drawer"
          onClick={toggleSideBar}
          sx={{
            marginRight: "25px",
            marginLeft: "20px",
            ...(sideBarOpen && { display: "none" }),
          }}
        >
          <MenuIcon />
        </IconButton>
        <Typography
          component="h1"
          variant="h6"
          color="inherit"
          noWrap
          sx={{ flexGrow: 1 }}
        >
          Dashboard
        </Typography>
        <Button
          variant="text"
          color="inherit"
          startIcon={<LogoutIcon />}
          sx={{ ml: 1 }}
          onClick={openModal}
        >
          LogOut
        </Button>
      </Toolbar>
      <LogoutModal closeModal={closeModal} modalOpen={modalOpen}/>
      <Backdrop
        sx={{
          color: "#fff",
          zIndex: (theme) => theme.zIndex.drawer + 1,
        }}
        open={isLoading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </AppBar>
  );
}
