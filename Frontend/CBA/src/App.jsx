import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import CssBaseline from "@mui/material/CssBaseline";
import { ToastContainer } from "react-toastify";

import Login from "./Pages/Login";
import Dashboard from "./Pages/Dashboard";
import Copyright from "./Components/Copyright";
import Users from "./Views/Users";
import UserRoles from "./Views/UserRoles";
import ForgotPassword from "./Pages/ForgotPassword";
import { Typography } from "@mui/material";


function App() {
  return (
    <>
      <ToastContainer autoClose={3000} hideProgressBar />
      <CssBaseline />
      <Router>
        <Routes>
          <Route path="/" element={<Navigate to={"/login"} replace />} />
          <Route path="login" element={<Login />} />
          <Route path="forgot-password" element={<ForgotPassword />} />
          <Route path="dashboard" element={<Dashboard />}>
            <Route
              path=""
              element={
                <Typography gutterBottom variant="h6" sx={{ mx: 5, my: 3 }}>
                  Welcome To the Dashboard!
                </Typography>
              }
            />
            <Route path="users" element={<Users />} />
            <Route path="user-roles" element={<UserRoles />} />
          </Route>
        </Routes>
      </Router>
      <Copyright sx={{ mt: 8, mb: 4 }} />
    </>
  );
}

export default App;
