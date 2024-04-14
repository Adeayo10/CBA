import { useState, useEffect } from "react";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Paper from "@mui/material/Paper";
import Link from "@mui/material/Link";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import IconButton from "@mui/material/IconButton";
import Button from "@mui/material/Button";
import AddIcon from "@mui/icons-material/Add";
import { Typography } from "@mui/material";

import Title from "../../Components/Title";

import { TOAST_CONFIG } from "../../utils/constants";
import { toast } from "react-toastify";

import { getUsers } from "../../api/users";

export default function DashboardHome() {
  const rolesList = ["SuperAdmin", "Admin", "Manager", "User"];

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>Welcome!</Title>
            <Typography variant="title"> Quick Navigate</Typography>
            <Grid
              container
              rowSpacing={1}
              columnSpacing={{ xs: 1, sm: 2, md: 3 }}
              sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
            >

            </Grid>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
