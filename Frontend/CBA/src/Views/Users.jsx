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
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import Backdrop from "@mui/material/Backdrop";
import CircularProgress from "@mui/material/CircularProgress";
import Box from "@mui/material/Box";
import RefreshIcon from "@mui/icons-material/Refresh";

import {
  Link as RouterLink,
  useNavigate,
  redirect,
  Navigate,
} from "react-router-dom";

import Title from "../Components/Title";
import UserCreateModal from "../Components/UserCreateModal";
import UserDetailsModal from "../Components/UserDetailsModal";

import { TOAST_CONFIG } from "../utils/constants";

import { toast } from "react-toastify";

import { getUsers } from "../api/users";
import { capitalize } from "../utils/util";
import { ErrorTwoTone } from "@mui/icons-material";
import { redirectIfRefreshTokenExpired } from "../api/auth";

export default function Users() {
  const [usersList, setUsersList] = useState([]);
  const [userBranchList, setUserBranchList] = useState([]);
  const [currentUserElement, setCurrentUserElement] = useState(null);
  const [currentUserDetails, setCurrentUserDetails] = useState({
    user: {},
    userBranch: {},
  });

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const menuOpen = Boolean(currentUserElement);

  const navigate = useNavigate()

  useEffect(() => {
    getUsers()
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.users)
          throw new Error(data.message || data.errors);

        // toast.success(data.message, TOAST_CONFIG);
        setUsersList(data.users);
        setUserBranchList(data.userBranch);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate)
      });

    return () => {
      setUsersList([]);
    };
  }, []);

  const openMenu = (event) => {
    setCurrentUserElement(event.currentTarget);
  };

  const closeMenu = () => {
    setCurrentUserElement(null);
  };

  const showUserInfo = (event) => {
    const userIndex = currentUserElement.name;
    const user = usersList[userIndex];
    const userBranch = userBranchList.filter(
      (branch) => branch.userId.toLowerCase() === user.id.toLowerCase()
    )[0];
    setCurrentUserDetails({ user, userBranch });
    toggleUserModal();
    closeMenu();
  };

  const toggleUserModal = () => {
    setDetailsModalOpen(!detailsModalOpen);
  };

  const toggleDisableUser = (event) => {
    const userIndex = currentUserElement.name;
    let usersListCopy = [...usersList];
    usersListCopy[userIndex].disabled = !usersListCopy[userIndex].disabled;
    setUsersList([...usersListCopy]);
    closeMenu();
  };

  const toggleCreateModal = () => {
    setCreateModalOpen(!createModalOpen);
  };

  const refreshUsers = () => {
    setIsLoading(true);
    getUsers()
      .then((data) => {
        console.log(data);
        if (data.errors || !data.users)
          throw new Error(data.message || data.errors);

        toast.success("Successfull", TOAST_CONFIG);
        setUsersList(data.users);
        setUserBranchList(data.userBranch);
        setIsLoading(false);
      })
      .catch((error) => {
        console.log(error);
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate)
      });
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>Users</Title>
            <Box sx={{ display: "flex", ml: "auto", mt: -5, mb: 2 }}>
              <Button
                variant="contained"
                startIcon={<RefreshIcon />}
                sx={{ ml: 1 }}
                onClick={refreshUsers}
              >
                Refresh
              </Button>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                sx={{ ml: 1 }}
                onClick={toggleCreateModal}
              >
                Add User
              </Button>
            </Box>
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
                  (
                    {
                      id,
                      fullName,
                      userName,
                      email,
                      phoneNumber,
                      role,
                      disabled,
                    },
                    index
                  ) => {
                    let splitName = fullName.split(" ");
                    let firstName = splitName[0];
                    let lastName =
                      splitName.length > 1 ? splitName[1] : "No Lastname";

                    firstName = capitalize(firstName);
                    lastName = capitalize(lastName);

                    const disabledText = disabled ? { color: "#575757" } : {};
                    const disabledRow = disabled
                      ? { backgroundColor: "#c9c9c9" }
                      : {};

                    return (
                      <TableRow key={id} style={disabledRow}>
                        <TableCell style={disabledText}>{firstName}</TableCell>
                        <TableCell style={disabledText}>{lastName}</TableCell>
                        <TableCell style={disabledText}>{userName}</TableCell>
                        <TableCell style={disabledText}>{email}</TableCell>
                        <TableCell style={disabledText}>
                          {phoneNumber}
                        </TableCell>
                        <TableCell style={disabledText}>{role}</TableCell>
                        <TableCell align="right">
                          <IconButton
                            aria-label="Show user actions"
                            onClick={openMenu}
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
            <Menu
              id="action-menu"
              anchorEl={currentUserElement}
              open={menuOpen}
              onClose={closeMenu}
              MenuListProps={{
                "aria-labelledby": "action-menu",
              }}
              anchorOrigin={{
                vertical: "top",
                horizontal: "left",
              }}
              transformOrigin={{
                vertical: "top",
                horizontal: "left",
              }}
            >
              <MenuItem onClick={showUserInfo}>View User</MenuItem>
              <MenuItem onClick={toggleDisableUser}>
                {currentUserElement &&
                usersList[currentUserElement.name].disabled
                  ? "Enable"
                  : "Disable"}{" "}
                User
              </MenuItem>
            </Menu>
            <UserDetailsModal
              modalOpen={detailsModalOpen}
              toggleModal={toggleUserModal}
              user={currentUserDetails.user}
              userBranch={currentUserDetails.userBranch}
            />
            <UserCreateModal
              toggleModal={toggleCreateModal}
              modalOpen={createModalOpen}
              refreshUsers={refreshUsers}
            />
            <Backdrop
              sx={{
                color: "#fff",
                zIndex: (theme) => theme.zIndex.drawer + 1,
              }}
              open={isLoading}
            >
              <CircularProgress color="inherit" />
            </Backdrop>
            <Link color="primary" href="#" onClick={null} sx={{ mt: 3 }}>
              See more users
            </Link>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
