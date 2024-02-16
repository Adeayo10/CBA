import { useState } from "react";
import { styled, createTheme, ThemeProvider } from "@mui/material/styles";
import Box from "@mui/material/Box";
import MuiAppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import List from "@mui/material/List";
import Typography from "@mui/material/Typography";
import Divider from "@mui/material/Divider";
import IconButton from "@mui/material/IconButton";
import Badge from "@mui/material/Badge";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Link from "@mui/material/Link";
import MenuIcon from "@mui/icons-material/Menu";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import NotificationsIcon from "@mui/icons-material/Notifications";
import { Outlet } from "react-router-dom";
import SideBar from "../Components/SideBar";
import {
  Link as RouterLink,
  useNavigate,
  redirect,
  Navigate,
} from "react-router-dom";

import { DRAWER_WIDTH } from "../utils/constants";
import Header from "../Components/Header";
import { tokenExists } from "../api/auth";

export default function Dashboard(props) {
  if (!tokenExists()) {
    return <Navigate to={"/login"} replace />;
  }

  const [sideBarOpen, setSideBarOpen] = useState(true);
  const toggleSideBar = () => {
    setSideBarOpen(!sideBarOpen);
  };

  return (
    <Box sx={{ display: "flex" }}>
      <Header sideBarOpen={sideBarOpen} toggleSideBar={toggleSideBar} />
      <SideBar sideBarOpen={sideBarOpen} toggleSideBar={toggleSideBar} />
      <Box
        component="main"
        sx={{
          backgroundColor: (theme) =>
            theme.palette.mode === "light"
              ? theme.palette.grey[100]
              : theme.palette.grey[900],
          flexGrow: 1,
          height: "100vh",
          overflow: "auto",
        }}
      >
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
}
