import { useState } from "react";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import { Outlet } from "react-router-dom";
import SideBar from "../Components/SideBar";
import { Navigate } from "react-router-dom";

import Header from "../Components/Header";
import { tokenExists } from "../utils/token";
import Copyright from "../Components/Copyright";

export default function Dashboard(props) {
  // if (!tokenExists()) {
  //   return <Navigate to={"/login"} replace />;
  // }

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
        <Copyright />
      </Box>
    </Box>
  );
}
