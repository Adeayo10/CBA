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

import { Link as RouterLink, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";

import { DRAWER_WIDTH, TOAST_CONFIG } from "../utils/constants";
import { logoutUser, clearTokenData } from "../api/auth";

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

  const logOut = (event) => {
    event.preventDefault();
    setIsLoading(true);
    logoutUser()
      .then((data) => {
        if (!data.ok || data.error) throw new Error(data.message || data.error);

        clearTokenData();
        setIsLoading(false);
        toast.success("Successfull", TOAST_CONFIG);
        navigate("/login");
      })
      .catch((error) => {
        setIsLoading(false);
        clearTokenData();
        navigate("/login");
        toast.error(error.message, TOAST_CONFIG);
      });
  };

  const viewProfile = () => {};

  return (
    <AppBar position="absolute" open={sideBarOpen}>
      <Toolbar
        sx={{
          pr: "24px", // keep right padding when drawer closed
        }}
      >
        <Avatar
          sx={{
            m: 1,
            bgcolor: "secondary.main",
            marginRight: "0px",
            marginLeft: "0px",
            ...(sideBarOpen && { display: "none" }),
          }}
        >
          <LockOutlinedIcon />
        </Avatar>
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
          onClick={logOut}
        >
          LogOut
        </Button>
      </Toolbar>
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
