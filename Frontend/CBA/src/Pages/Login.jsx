import { useState } from "react";
import { toast } from "react-toastify";
import { Link as RouterLink, useNavigate, Navigate } from "react-router-dom";

import IconButton from "@mui/material/IconButton";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import FormControlLabel from "@mui/material/FormControlLabel";
import Checkbox from "@mui/material/Checkbox";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import Link from "@mui/material/Link";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";

import { loginUser } from "../api/auth";
import { tokenExists, saveTokenData } from "../utils/token";
import { TOAST_CONFIG } from "../utils/constants";
import Copyright from "../Components/Copyright";

export default function Login() {
  const navigate = useNavigate();
  const [loginDetails, setLoginDetails] = useState({ Email: "", Password: "" });
  const [showPassword, setShowPassword] = useState(false);

  const [isLoading, setIsLoading] = useState(false);

  if (tokenExists()) {
    //console.log("Here");
    return <Navigate to={"/dashboard"} replace />;
  }

  function handleSubmit(submitEvent) {
    submitEvent.preventDefault();
    setIsLoading(true);
    loginUser({ ...loginDetails })
      .then((data) => {
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        saveTokenData(data.token, data.refreshToken, data.expiryDate);
        setIsLoading(false);
        toast.success(data.message, TOAST_CONFIG);
        navigate("/dashboard");
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
      });
  }

  function handleChange(changeEvent) {
    changeEvent.persist();
    const { name, value } = changeEvent.target;
    setLoginDetails({ ...loginDetails, [name]: value });
  }

  function togglePasswordVisibility(clickEvent) {
    clickEvent.preventDefault();
    setShowPassword((prevState) => !prevState);
  }

  return (
    <Container component="main" maxWidth="xs">
      <CssBaseline />
      <Box
        sx={{
          marginTop: 8,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Avatar sx={{ m: 1, bgcolor: "secondary.main" }}>
          <LockOutlinedIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Sign in
        </Typography>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="Email"
            autoComplete="email"
            autoFocus
            onChange={handleChange}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="Password"
            label="Password"
            type={showPassword ? "text" : "password"}
            id="password"
            autoComplete="current-password"
            onChange={handleChange}
            InputProps={{
              endAdornment: (
                <IconButton
                  aria-label="toggle password visibility"
                  onClick={togglePasswordVisibility}
                  edge="end"
                >
                  {showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              ),
            }}
          />
          {/* <FormControlLabel
            control={<Checkbox value="remember" color="primary" />}
            label="Remember me"
          /> */}
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Sign In
          </Button>
          <Grid container>
            <Grid item xs>
              <Link
                to={"/forgot-password"}
                component={RouterLink}
                variant="body2"
              >
                Forgot password?
              </Link>
            </Grid>
          </Grid>
          <Backdrop
            sx={{
              color: "#fff",
              zIndex: (theme) => theme.zIndex.drawer + 1,
            }}
            open={isLoading}
          >
            <CircularProgress color="inherit" />
          </Backdrop>
        </Box>
      </Box>
      <Copyright />
    </Container>
  );
}
