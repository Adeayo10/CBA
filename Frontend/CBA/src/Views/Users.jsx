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
import UserUpdateModal from "../Components/UserUpdateModal";

import {
  TOAST_CONFIG,
  USER_STATUS,
  CREATE_USER_BASE,
  CREATE_USER_BRANCH_BASE,
} from "../utils/constants";

import { toast } from "react-toastify";

import { forgotPassword } from "../api/auth";
import { getUsers, deactivateUser, activateUser } from "../api/users";
import { capitalize, extractUpdateFields } from "../utils/util";
import { ErrorTwoTone } from "@mui/icons-material";
import { redirectIfRefreshTokenExpired } from "../utils/token";

export default function Users() {
  const [usersList, setUsersList] = useState([]);
  const [userBranchList, setUserBranchList] = useState([]);
  const [currentUserElement, setCurrentUserElement] = useState(null);
  const [currentUserDetails, setCurrentUserDetails] = useState({
    user: {},
    userBranch: {},
  });

  const [currentUpdateUser, setCurrentUpdateUser] = useState({
    user: {},
    userBranch: {},
  });

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const menuOpen = Boolean(currentUserElement);
  const navigate = useNavigate();

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
        redirectIfRefreshTokenExpired(error.message, navigate);
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
    event.preventDefault();
    const userIndex = currentUserElement.name;
    const user = usersList[userIndex];
    const userBranch = userBranchList.filter(
      (branch) => branch.userId.toLowerCase() === user.id.toLowerCase()
    )[0];
    setCurrentUserDetails({ user, userBranch });
    toggleDetailsModal();
    closeMenu();
  };

  const showUpdateModal = (event) => {
    event.preventDefault();
    const userIndex = currentUserElement.name;
    if (!userIndex) {
      toast.error("Unable to Get User", TOAST_CONFIG);
      return;
    }
    const user = extractUpdateFields(usersList[userIndex], CREATE_USER_BASE, [
      "password",
    ]);
    const userBranch = extractUpdateFields(
      userBranchList.filter(
        (branch) => branch.userId.toLowerCase() === user.id.toLowerCase()
      )[0],
      CREATE_USER_BRANCH_BASE
    );

    // console.log({user})
    // console.log({userBranch})
    setCurrentUpdateUser({ user, userBranch });
    toggleUpdateModal();
    closeMenu();
  };

  const toggleDetailsModal = () => {
    setDetailsModalOpen(!detailsModalOpen);
  };

  const toggleCreateModal = () => {
    setCreateModalOpen(!createModalOpen);
  };

  const toggleUpdateModal = () => {
    setUpdateModalOpen(!updateModalOpen);
  };

  const disableUser = (event) => {
    const userIndex = currentUserElement.name;
    const userId = usersList[userIndex]?.id;

    if (!userId) {
      toast.error("Invalid User Selected", TOAST_CONFIG);
      return;
    }

    event.preventDefault();
    setIsLoading(true);
    deactivateUser(userId)
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.success)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshUsersList();
      })
      .catch((error) => {
        //console.log(error);
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });

    closeMenu();
  };

  const enableUser = (event) => {
    const userIndex = currentUserElement.name;
    const userId = usersList[userIndex]?.id;

    if (!userId) {
      toast.error("Invalid User Selected", TOAST_CONFIG);
      return;
    }

    event.preventDefault();
    setIsLoading(true);
    activateUser(String(userId))
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.success)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshUsersList();
      })
      .catch((error) => {
        //console.log(error);
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });

    closeMenu();
  };

  const refreshUsersList = () => {
    setIsLoading(true);
    getUsers()
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.users)
          throw new Error(data.message || data.errors);

        // toast.success("Successfull", TOAST_CONFIG);
        setUsersList(data.users);
        setUserBranchList(data.userBranch);
        setIsLoading(false);
      })
      .catch((error) => {
        //console.log(error);
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  };

  const resetUserPassword = (event) => {
    event.preventDefault();

    const userIndex = currentUserElement.name;
    if (!userIndex) {
      toast.error("Unable to Get User", TOAST_CONFIG);
      return;
    }
    const email = usersList[userIndex]?.email;

    if (!email) {
      toast.error("Unable to Get User", TOAST_CONFIG);
      return;
    }

    setIsLoading(true);

    forgotPassword(email)
      .then((data) => {
        //console.log(data);
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        setIsLoading(false);
        toast.success(data.message, TOAST_CONFIG);
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
      });
    closeMenu();
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
                onClick={refreshUsersList}
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
                  <TableCell>Status</TableCell>
                  <TableCell>Branch</TableCell>
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
                      status,
                    },
                    index
                  ) => {
                    let splitName = fullName.split(" ");
                    let firstName = splitName[0];
                    let lastName =
                      splitName.length > 1 ? splitName[1] : "No Lastname";

                    firstName = capitalize(firstName);
                    lastName = capitalize(lastName);

                    const disabledText =
                      status !== USER_STATUS.ACTIVE ? { color: "#575757" } : {};
                    const disabledRow =
                      status !== USER_STATUS.ACTIVE
                        ? { backgroundColor: "#c9c9c9" }
                        : {};

                    const userBranch = userBranchList.filter(
                      (branch) =>
                        branch.userId.toLowerCase() === id.toLowerCase()
                    )[0];

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
                        <TableCell style={disabledText}>{status}</TableCell>
                        <TableCell style={disabledText}>
                          {userBranch.name}
                        </TableCell>
                        <TableCell align="right">
                          <IconButton
                            aria-label="Show user actions"
                            onClick={openMenu}
                            name={index}
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
              {currentUserElement !== null &&
              currentUserElement.name !== undefined
                ? usersList[currentUserElement.name].status ==
                  USER_STATUS.ACTIVE
                  ? [
                      <MenuItem
                        onClick={showUserInfo}
                        key={`${currentUserElement.name}_view_enabled`}
                      >
                        View User
                      </MenuItem>,
                      <MenuItem
                        onClick={resetUserPassword}
                        key={`${currentUserElement.name}_reset`}
                      >
                        Reset Password
                      </MenuItem>,
                      <MenuItem
                        onClick={showUpdateModal}
                        key={`${currentUserElement.name}_update`}
                      >
                        Uptate User
                      </MenuItem>,
                      <MenuItem
                        onClick={disableUser}
                        key={`${currentUserElement.name}_disable`}
                      >
                        Disable User
                      </MenuItem>,
                    ]
                  : [
                      <MenuItem
                        onClick={showUserInfo}
                        key={`${currentUserElement.name}_view_disabled`}
                      >
                        View User
                      </MenuItem>,
                      <MenuItem
                        onClick={enableUser}
                        key={`${currentUserElement.name}_enable`}
                      >
                        Enable User
                      </MenuItem>,
                    ]
                : ""}
            </Menu>
            <UserDetailsModal
              modalOpen={detailsModalOpen}
              toggleModal={toggleDetailsModal}
              user={currentUserDetails.user}
              userBranch={currentUserDetails.userBranch}
            />
            <UserCreateModal
              toggleModal={toggleCreateModal}
              modalOpen={createModalOpen}
              refreshUsersList={refreshUsersList}
            />
            <UserUpdateModal
              modalOpen={updateModalOpen}
              toggleModal={toggleUpdateModal}
              user={currentUpdateUser.user}
              userBranch={currentUpdateUser.userBranch}
              refreshUsersList={refreshUsersList}
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
