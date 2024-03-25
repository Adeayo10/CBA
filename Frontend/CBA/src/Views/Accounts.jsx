import { useState, useEffect, useMemo } from "react";
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
import Pagination from "@mui/material/Pagination";

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
  STATUS as STATUS,
  CREATE_USER_BASE,
  CREATE_USER_BRANCH_BASE,
  CREATE_ACCOUNT_BASE
} from "../utils/constants";

import { toast } from "react-toastify";

import { forgotPassword } from "../api/auth";
import { getUsers, deactivateUser, activateUser } from "../api/users";
import { changeCustomerAccountStatus, getCustomers } from "../api/customer";
import { capitalize, extractUpdateFields } from "../utils/util";
import { ErrorTwoTone } from "@mui/icons-material";
import { redirectIfRefreshTokenExpired } from "../utils/token";
import AccountUpdateModal from "../Components/AccountUpdateModal";
import AccountCreateModal from "../Components/AccountCreateModal";

export default function Accounts({ accountType }) {
  const [accountsList, setAccountsList] = useState([]);
  const [currentAccountElement, setCurrentAccountElement] = useState(null);

  const [currentAccountDetails, setCurrentAccountDetails] = useState({});

  const [currentUpdateAccount, setCurrentUpdateAccount] = useState({});

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const [currentPage, setCurrentPage] = useState(1);

  const menuOpen = Boolean(currentAccountElement);
  const navigate = useNavigate();

  useEffect(() => {
    getCustomers(accountType)
      .then((data) => {
        console.log(data);
        if (data.errors || !data.length)
          throw new Error(data.message || data.errors);

        setAccountsList(data);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });

    return () => {
      setAccountsList([]);
    };
  }, [accountType]);

  const openMenu = (event) => {
    setCurrentAccountElement(event.currentTarget);
  };

  const closeMenu = () => {
    setCurrentAccountElement(null);
  };

  const showAccountInfo = (event) => {
    event.preventDefault();
    const accountIndex = currentAccountElement.name;
    const account = accountsList[accountIndex];
    setCurrentAccountDetails( account);
    toggleDetailsModal();
    closeMenu();
  };

  const showUpdateModal = (event) => {
    event.preventDefault();
    const accountIndex = currentAccountElement.name;
    if (!accountIndex) {
      toast.error("Unable to Get Account", TOAST_CONFIG);
      return;
    }
    const account = extractUpdateFields(
      accountsList[accountIndex],
      CREATE_ACCOUNT_BASE,
    );

    // console.log({user})
    // console.log({userBranch})
    setCurrentUpdateAccount(account);
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

  const toggleAccountStatus = (event) => {
    const accountIndex = currentAccountElement.name;
    const accountId = accountsList[accountIndex]?.id;

    if (!accountId) {
      toast.error("Invalid User Selected", TOAST_CONFIG);
      return;
    }

    event.preventDefault();
    setIsLoading(true);
    changeCustomerAccountStatus(accountId)
      .then((data) => {
        console.log(data);
        if (data.errors )
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshAccountList();
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });

    closeMenu();
  };


  const refreshAccountList = () => {
    setIsLoading(true);
    getCustomers(accountType, currentPage)
      .then((data) => {
        console.log(data);
        if (data.errors || !data.length)
          throw new Error(data.message || data.errors);

        setAccountsList(data);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  };

  const showAccountBalance = (event) => {
    event.preventDefault();

    const accountIndex = currentAccountElement.name;
    if (!accountIndex) {
      toast.error("Unable to Get Account", TOAST_CONFIG);
      return;
    }
    const balance = accountsList[accountIndex]?.balance;

    if (balance == null) {
      toast.error("Unable to Get Balance", TOAST_CONFIG);
      return;
    }
    closeMenu();
  };

  const handlePageChange = (event, page) => {
    setCurrentPage(page);
    setIsLoading(true);
    getCustomers(accountType, page)
      .then((data) => {
        console.log(data);
        if (data.errors || !data.length)
          throw new Error(data.message || data.errors);

        setAccountsList(data);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>{accountType} Accounts</Title>
            <Box sx={{ display: "flex", ml: "auto", mt: -5, mb: 2 }}>
              <Button
                variant="contained"
                startIcon={<RefreshIcon />}
                sx={{ ml: 1 }}
                onClick={refreshAccountList}
              >
                Refresh
              </Button>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                sx={{ ml: 1 }}
                onClick={toggleCreateModal}
              >
                Add Account
              </Button>
            </Box>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Firstname</TableCell>
                  <TableCell>Lastname</TableCell>
                  <TableCell>AccountNo</TableCell>
                  <TableCell>Type</TableCell>
                  <TableCell>Branch</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {accountsList.map(
                  ({ id, fullName, accountNumber, status, branch }, index) => {
                    let splitName = fullName.split(" ");
                    let firstName = splitName[0];
                    let lastName =
                      splitName.length > 1 ? splitName[1] : "No Lastname";

                    firstName = capitalize(firstName);
                    lastName = capitalize(lastName);

                    const disabledText =
                      status !== STATUS.ACTIVE ? { color: "#575757" } : {};
                    const disabledRow =
                      status !== STATUS.ACTIVE
                        ? { backgroundColor: "#c9c9c9" }
                        : {};

                    return (
                      <TableRow key={id} style={disabledRow}>
                        <TableCell style={disabledText}>{firstName}</TableCell>
                        <TableCell style={disabledText}>{lastName}</TableCell>

                        <TableCell style={disabledText}>
                          {accountNumber}
                        </TableCell>
                        <TableCell style={disabledText}>
                          {accountType}
                        </TableCell>

                        <TableCell style={disabledText}>{branch}</TableCell>
                        <TableCell style={disabledText}>{status}</TableCell>
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
              anchorEl={currentAccountElement}
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
              {currentAccountElement !== null &&
              currentAccountElement.name !== undefined
                ? accountsList[currentAccountElement.name].status ==
                  STATUS.ACTIVE
                  ? [
                      <MenuItem
                        onClick={showAccountInfo}
                        key={`${currentAccountElement.name}_view_enabled`}
                      >
                        View Account
                      </MenuItem>,
                      <MenuItem
                        onClick={showAccountBalance}
                        key={`${currentAccountElement.name}_reset`}
                      >
                        View Balance
                      </MenuItem>,
                      <MenuItem
                      onClick={showUpdateModal}
                      key={`${currentAccountElement.name}_update_enabled`}
                    >
                      Update Account
                    </MenuItem>,
                      <MenuItem
                        onClick={toggleAccountStatus}
                        key={`${currentAccountElement.name}_disable`}
                      >
                        Deactivate Account
                      </MenuItem>,
                    ]
                  : [
                      <MenuItem
                        onClick={showAccountInfo}
                        key={`${currentAccountElement.name}_view_disabled`}
                      >
                        View Account
                      </MenuItem>,
                      <MenuItem
                        onClick={toggleAccountStatus}
                        key={`${currentAccountElement.name}_enable`}
                      >
                        Activate Account
                      </MenuItem>,
                    ]
                : ""}
            </Menu>

            <AccountCreateModal
              toggleModal={toggleCreateModal}
              modalOpen={createModalOpen}
              accountType={accountType}
              refreshAccountsList={refreshAccountList}
            />
            <AccountUpdateModal
              modalOpen={updateModalOpen}
              toggleModal={toggleUpdateModal}
              account={currentUpdateAccount}
              accountType={accountType}
              refreshAccountsList={refreshAccountList}
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
            <Pagination
              sx={{ mt: 3, ml: "auto" }}
              count={10}
              variant="outlined"
              shape="rounded"
              onChange={handlePageChange}
            />
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
