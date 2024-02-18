import { useState } from "react";
import { toast } from "react-toastify";
import { useNavigate, Navigate } from "react-router-dom";

import IconButton from "@mui/material/IconButton";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";
import PasswordIcon from '@mui/icons-material/Password';

import { resetPassword } from "../api/auth";
import { tokenExists } from "../utils/token";
import { TOAST_CONFIG } from "../utils/constants";
import Copyright from "../Components/Copyright";

export default function ResetPassword() {
  const navigate = useNavigate();
  const [formDetails, setFormDetails] = useState({
    password: "",
    confirmPassword: "",
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  if (tokenExists()) {
    console.log("Here");
    return <Navigate to={"/dashboard"} replace />;
  }

  function validateField(event) {
    event.preventDefault()
    formContainsErrors()
  }

  function formContainsErrors() {
    const { password, confirmPassword } = formDetails;

    if (!password)
      setFormErrors({ ...formErrors, password: "Field Cannot be Empty" });
    else if (!confirmPassword)
      setFormErrors({
        ...formErrors,
        confirmPassword: "Field Cannot be Empty",
      });
    else if (password !== confirmPassword)
      setFormErrors({
        password: "Passwords don't match",
        confirmPassword: "Passwords don't match",
      });
    else setFormErrors({});

    return !password || !confirmPassword || password !== confirmPassword;
  }

  function handleSubmit(submitEvent) {
    submitEvent.preventDefault();

    const { password } = formDetails;

    if (formContainsErrors()) {
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    setIsLoading(true);
    const queryParams = new URLSearchParams(window.location.search);

    const requestBody = {
      UserId: queryParams.get("userId"),
      Token: encodeURIComponent(queryParams.get("token")),
      Password: password,
    };

    resetPassword(requestBody)
      .then((data) => {
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        setIsLoading(false);
        toast.success(data.message, TOAST_CONFIG);
        navigate("/login");
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
      });
  }

  function handleChange(changeEvent) {
    changeEvent.persist();
    const { name, value } = changeEvent.target;
    setFormDetails({ ...formDetails, [name]: value });
  }

  function togglePasswordVisibility(clickEvent) {
    clickEvent.preventDefault();
    setShowPassword((prevState) => !prevState);
  }

  function toggleConfirmPasswordVisibility(clickEvent) {
    clickEvent.preventDefault();
    setShowConfirmPassword((prevState) => !prevState);
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
          <PasswordIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Reset Password
        </Typography>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type={showPassword ? "text" : "password"}
            id="password"
            onChange={handleChange}
            error={Boolean(formErrors.password)}
            helperText={formErrors.password}
            onBlur={validateField}
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
          <TextField
            margin="normal"
            required
            fullWidth
            name="confirmPassword"
            label="Confirm Password"
            type={showConfirmPassword ? "text" : "password"}
            id="confirmPassword"
            onChange={handleChange}
            error={Boolean(formErrors.confirmPassword)}
            helperText={formErrors.confirmPassword}
            onBlur={validateField}
            InputProps={{
              endAdornment: (
                <IconButton
                  aria-label="toggle password visibility"
                  onClick={toggleConfirmPasswordVisibility}
                  edge="end"
                >
                  {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
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
            Reset Password
          </Button>
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
