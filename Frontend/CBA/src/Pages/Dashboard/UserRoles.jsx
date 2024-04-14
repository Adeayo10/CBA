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

import Title from "../../Components/Title";

import { TOAST_CONFIG } from "../../utils/constants";
import { toast } from "react-toastify";

import { getUsers } from "../../api/users";

export default function UserRoles() {
  const rolesList = ["SuperAdmin", "Admin", "Manager", "User"];

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>User Roles</Title>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Role Id</TableCell>
                  <TableCell>Role Name</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {rolesList.map((roleName, index) => {
                  let roleId = index + 1;
                  let roleKey = `${roleName}_${roleId}`;
                  return (
                    <TableRow key={roleKey}>
                      <TableCell>{roleId}</TableCell>
                      <TableCell>{roleName}</TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
