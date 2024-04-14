import { useState } from "react";
import { toast } from "react-toastify";
import { useNavigate, Navigate } from "react-router-dom";

import IconButton from "@mui/material/IconButton";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";

import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import Person2Icon from "@mui/icons-material/Person2";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";

import Title from "../../Components/Title";

import { resetPassword, changePassword } from "../../api/auth";
import {
  tokenExists,
  retrieveUserFromToken,
  redirectIfRefreshTokenExpired,
} from "../../utils/token";
import { TOAST_CONFIG } from "../../utils/constants";
import Copyright from "../../Components/Copyright";

export default function Profile() {
  const navigate = useNavigate();
  const [loggedInUser, setLoggedInUser] = useState(retrieveUserFromToken());
  const [formDetails, setFormDetails] = useState({
    currentPassword: "",
    newPassword: "",
    confirmNewPassword: "",
  });

  const [passwordVisibility, setPasswordVisibility] = useState({
    showCurrentPassword: false,
    showNewPassword: false,
    showConfirmNewPassword: false,
  });

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  if (!tokenExists()) {
    //console.log("Here");
    return <Navigate to={"/login"} replace />;
  }

  function validateField(event) {
    event.preventDefault();
    formContainsErrors();
  }

  function formContainsErrors() {
    const { currentPassword, newPassword, confirmNewPassword } = formDetails;

    if (!currentPassword)
      setFormErrors({
        ...formErrors,
        currentPassword: "Field Cannot be Empty",
      });
    else if (currentPassword === newPassword)
      setFormErrors({
        ...formErrors,
        currentPassword: "Old and New Passwords Cannot be the Same",
        newPassword: "Old and New Passwords Cannot be the Same",
      });
    else if (!newPassword)
      setFormErrors({ ...formErrors, newPassword: "Field Cannot be Empty" });
    else if (!confirmNewPassword)
      setFormErrors({
        ...formErrors,
        confirmNewPassword: "Field Cannot be Empty",
      });
    else if (newPassword !== confirmNewPassword)
      setFormErrors({
        newPassword: "Passwords don't match",
        confirmNewPassword: "Passwords don't match",
      });
    else setFormErrors({});

    return (
      !currentPassword ||
      !newPassword ||
      !confirmNewPassword ||
      newPassword !== confirmNewPassword
    );
  }

  function handleSubmit(submitEvent) {
    submitEvent.preventDefault();

    const { currentPassword, newPassword } = formDetails;

    if (formContainsErrors()) {
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    setIsLoading(true);

    const requestBody = {
      email: loggedInUser.email,
      currentPassword,
      newPassword,
    };

    changePassword(requestBody)
      .then((data) => {
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        setIsLoading(false);
        toast.success(data.message, TOAST_CONFIG);
        navigate("/dashboard");
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  }

  function handleChange(changeEvent) {
    changeEvent.persist();
    const { name, value } = changeEvent.target;
    setFormDetails({ ...formDetails, [name]: value });
  }

  function togglePasswordVisibility(clickEvent) {
    const { name } = clickEvent.currentTarget;
    setPasswordVisibility((prevState) => {
      return { ...prevState, [name]: !prevState[name] };
    });
  }

  return (
    <Container component="main" maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Box
              sx={{
                my: 3,
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
              }}
            >
              <Typography
                component="h2"
                variant="h5"
                color="primary"
                gutterBottom
              >
                Change Password
              </Typography>

              <Box
                component="form"
                onSubmit={handleSubmit}
                noValidate
                sx={{ mt: 1 }}
              >
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  name="username"
                  label="Username"
                  type="text"
                  id="username"
                  value={loggedInUser.username}
                  disabled
                />
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  name="email"
                  label="Email"
                  type="text"
                  id="email"
                  value={loggedInUser.email}
                  disabled
                />
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  name="currentPassword"
                  label="Current Password"
                  type={
                    passwordVisibility.showCurrentPassword ? "text" : "password"
                  }
                  id="currentPassword"
                  autoComplete="current-password"
                  onChange={handleChange}
                  error={Boolean(formErrors.currentPassword)}
                  helperText={formErrors.currentPassword}
                  onBlur={validateField}
                  InputProps={{
                    endAdornment: (
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={togglePasswordVisibility}
                        name="showCurrentPassword"
                        edge="end"
                      >
                        {passwordVisibility.showCurrentPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    ),
                  }}
                />
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  name="newPassword"
                  label="New Password"
                  type={
                    passwordVisibility.showNewPassword ? "text" : "password"
                  }
                  id="newPassword"
                  onChange={handleChange}
                  error={Boolean(formErrors.newPassword)}
                  helperText={formErrors.newPassword}
                  onBlur={validateField}
                  InputProps={{
                    endAdornment: (
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={togglePasswordVisibility}
                        name="showNewPassword"
                        edge="end"
                      >
                        {passwordVisibility.showNewPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    ),
                  }}
                />
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  name="confirmNewPassword"
                  label="Confirm New Password"
                  type={
                    passwordVisibility.showConfirmNewPassword
                      ? "text"
                      : "password"
                  }
                  id="confirmNewPassword"
                  onChange={handleChange}
                  error={Boolean(formErrors.confirmNewPassword)}
                  helperText={formErrors.confirmNewPassword}
                  onBlur={validateField}
                  InputProps={{
                    endAdornment: (
                      <IconButton
                        aria-label="toggle password visibility"
                        onClick={togglePasswordVisibility}
                        name="showConfirmNewPassword"
                        edge="end"
                      >
                        {passwordVisibility.showConfirmNewPassword ? (
                          <VisibilityOff />
                        ) : (
                          <Visibility />
                        )}
                      </IconButton>
                    ),
                  }}
                />
                <Button
                  type="submit"
                  fullWidth
                  variant="contained"
                  sx={{ mt: 3, mb: 2 }}
                >
                  Change Password
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
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
