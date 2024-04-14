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
import DoNotDisturbIcon from "@mui/icons-material/DoNotDisturb";
import Copyright from "../Components/Copyright";
import { ROUTES } from "../utils/constants";

export default function NotFound() {
  console.log(document.referrer);
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
          <DoNotDisturbIcon />
        </Avatar>
        <Typography component="h1" variant="h5">
          Page Not Found
        </Typography>
      </Box>
      <Grid sx={{ textAlign: "center", m: 1 }}>
        <Link to={ROUTES.DASHBOARD} component={RouterLink} variant="body2">
          Go to Dashboard
        </Link>
      </Grid>
      <Copyright />
    </Container>
  );
}
