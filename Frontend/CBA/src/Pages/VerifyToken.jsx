import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import { Link as RouterLink, useNavigate, Navigate } from "react-router-dom";

import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";
import PasswordIcon from "@mui/icons-material/Password";

import { verifyCode, resendCode } from "../api/auth";
import { saveTokenData, tokenExists } from "../utils/token";
import { ROUTES, TOAST_CONFIG } from "../utils/constants";
import Copyright from "../Components/Copyright";

export default function VerifyToken() {
  const navigate = useNavigate();
  const [code, setCode] = useState("");
  const [codeError, setCodeError] = useState("");

  const [isLoading, setIsLoading] = useState(false);

  if (tokenExists()) {
    return <Navigate to={ROUTES.DASHBOARD} replace />;
  }

  useEffect(() => {
    const queryParams = new URLSearchParams(window.location.search);
    const userId = queryParams.get("userId");
    const code = queryParams.get("token");

    if (userId && code) {
      verifyCode(code, userId)
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
  }, []);

  const handleSubmit = (submitEvent) => {
    submitEvent.preventDefault();
    if (!code) {
      validateCode({ target: { value: code } });
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }
    setIsLoading(true);
    verifyCode(code)
      .then((data) => {
        console.log(data);
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        saveTokenData(data.token, data.refreshToken, data.expiryDate);
        setIsLoading(false);
        toast.success("Login Successful!", TOAST_CONFIG);
        navigate(ROUTES.DASHBOARD);
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
      });
  };

  const handleChange = (changeEvent) => {
    changeEvent.persist();
    const { name, value } = changeEvent.target;
    setCode(value);
  };

  const validateCode = (event) => {
    const { name, value } = event.target;
    if (!value) setCodeError("Field Cannot Be Empty");
    else setCodeError("");
  };

  const handleResendToken = (event) => {
    event.preventDefault();
    setIsLoading(true);
    resendCode()
      .then((data) => {
        console.log(data);
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        setIsLoading(false);
        toast.success(data.message, TOAST_CONFIG);
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
      });
  };

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
          Enter Verification Code
        </Typography>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="code"
            label="Verification Code"
            name="code"
            autoFocus
            onChange={handleChange}
            onBlur={validateCode}
            error={Boolean(codeError)}
            helperText={codeError}
          />

          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            Submit
          </Button>
          <Grid container>
            <Grid item xs>
              <Link variant="body2" type="button" onClick={handleResendToken}>
                Did not get a verification token?
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
