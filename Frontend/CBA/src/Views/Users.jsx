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

import Title from "../Components/Title";

import { TOAST_CONFIG } from "../util";
import { toast } from "react-toastify";

import { getUsers } from "../Api/users";

export default function Users() {
  const [usersList, setUsersList] = useState([]);

  useEffect(() => {
    getUsers()
      .then((data) => {
        console.log(data);
        if (data.error) throw new Error(data.message || data.error);

        toast.success(data.message, TOAST_CONFIG);
        setUsersList(data.users);
      })
      .catch((error) => {
        console.log(error);
        toast.error(error.message, TOAST_CONFIG);
      });

    return () => {
      setUsersList([]);
    };
  }, []);

  function showUser(event) {
    event.preventDefault();
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>User Management</Title>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Firstname</TableCell>
                  <TableCell>Lastname</TableCell>
                  <TableCell>Username</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Phone</TableCell>
                  <TableCell>Permission</TableCell>
                  <TableCell align="right">Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {usersList.map(
                  ({ id, fullName, userName, email, phoneNumber,role }, index) => {
                    let splitName = fullName.split(" ");
                    let firstName = splitName[0];
                    let lastName =
                      splitName.length > 1 ? splitName[1] : "No Lastname";

                    firstName = firstName[0].toUpperCase() + firstName.slice(1);
                    lastName = lastName[0].toUpperCase() + lastName.slice(1);
                    return (
                      <TableRow key={id}>
                        <TableCell>{firstName}</TableCell>
                        <TableCell>{lastName}</TableCell>
                        <TableCell>{userName}</TableCell>
                        <TableCell>{email}</TableCell>
                        <TableCell>{phoneNumber}</TableCell>
                        <TableCell>{role}</TableCell>
                        <TableCell align="right">
                          <IconButton
                            aria-label="toggle password visibility"
                            onClick={showUser}
                            name={index}
                            edge="end"
                          >
                            <MoreVertIcon />
                          </IconButton>
                        </TableCell>
                      </TableRow>
                    );
                  }
                )}
              </TableBody>
            </Table>
            <Link color="primary" href="#" onClick={showUser} sx={{ mt: 3 }}>
              See more users
            </Link>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
