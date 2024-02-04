import {
  BrowserRouter as Router,
  Route,
  Routes,
  redirect,
  useNavigate,
} from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import CssBaseline from "@mui/material/CssBaseline";
import { ToastContainer } from "react-toastify";

import Login from "./Pages/Login";
import Dashboard from "./Pages/Dashboard";
import Copyright from "./Components/Copyright";
import Users from "./Views/Users";
import { useEffect } from "react";

function RedirectToLogin(){
  const navigate = useNavigate()
  useEffect(()=>{
    navigate("/login")
  }, [])
  return (<></>)
}

function App() {
  return (
    <>
      <ToastContainer autoClose={3000} hideProgressBar />
      <CssBaseline />
      <Router>
        <Routes>
          <Route path="/" element={<RedirectToLogin />} />
          <Route path="login" element={<Login />} />
          <Route path="dashboard" element={<Dashboard />}>
            <Route path="" element={<Users />} />
            <Route path="users" element={<Users />} />
            <Route path="roles" element={<Login />} />
          </Route>
        </Routes>
      </Router>
      <Copyright sx={{ mt: 8, mb: 4 }} />
    </>
  );
}

export default App;
