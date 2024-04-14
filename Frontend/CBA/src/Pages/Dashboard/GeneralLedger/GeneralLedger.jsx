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

import { useNavigate } from "react-router-dom";

import Title from "../../../Components/Title";

import {
  TOAST_CONFIG,
  STATUS,
  PAGE_SIZE,
  CREATE_LEDGER_BASE,
} from "../../../utils/constants";

import { toast } from "react-toastify";

import { capitalize, extractUpdateFields } from "../../../utils/util";
import { redirectIfRefreshTokenExpired } from "../../../utils/token";
import { changeLedgerStatus, getLedgers } from "../../../api/ledger";
import LedgerCreateModal from "./LedgerCreateModal";
import LedgerUpdateModal from "./LedgerUpdateModal";
import LedgerDetailsModal from "./LedgerDetailsModal";

export default function GeneralLedger() {
  const [ledgersList, setLedgersList] = useState([]);
  const [currentLedgerElement, setCurrentLedgerElement] = useState(null);

  const [currentLedgerDetails, setCurrentLedgerDetails] = useState({});

  const [currentUpdateLedger, setCurrentUpdateLedger] = useState({});

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [updateModalOpen, setUpdateModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const [currentPage, setCurrentPage] = useState(1);
  const [noOfPages, setNoOfPages] = useState(1);

  const menuOpen = Boolean(currentLedgerElement);
  const navigate = useNavigate();

  useEffect(() => {
    fetchLedgers();
    return () => {
      setLedgersList([]);
    };
  }, []);

  const fetchLedgers = async (pageNumber = currentPage) => {
    setIsLoading(true);
    getLedgers(pageNumber)
      .then((data) => {
        console.log(data);
        if (!data.status) throw new Error(data.message || data.errors);

        setLedgersList(data.dataList);
        setNoOfPages(Math.ceil(data.totalRowCount / PAGE_SIZE));
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  };

  const openMenu = (event) => {
    setCurrentLedgerElement(event.currentTarget);
  };

  const closeMenu = () => {
    setCurrentLedgerElement(null);
  };

  const showLedgerAccountInfo = (event) => {
    event.preventDefault();
    const ledgerIndex = currentLedgerElement.name;
    const ledger = ledgersList[ledgerIndex];
    console.log(ledger);
    setCurrentLedgerDetails(ledger);
    toggleDetailsModal();
    closeMenu();
  };

  const showUpdateModal = (event) => {
    event.preventDefault();
    const ledgerIndex = currentLedgerElement.name;
    if (!ledgerIndex) {
      toast.error("Unable to Get Account", TOAST_CONFIG);
      return;
    }
    const ledger = extractUpdateFields(
      ledgersList[ledgerIndex],
      CREATE_LEDGER_BASE
    );
    setCurrentUpdateLedger(ledger);
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

  const toggleLedgerStatus = (event) => {
    const ledgerIndex = currentLedgerElement.name;
    const ledgerId = ledgersList[ledgerIndex]?.id;

    if (!ledgerId) {
      toast.error("Invalid Ledger Selected", TOAST_CONFIG);
      return;
    }

    event.preventDefault();
    setIsLoading(true);
    changeLedgerStatus(ledgerId)
      .then((data) => {
        console.log(data);
        if (!data.ok) throw new Error(data.message || data.errors);

        toast.success("Status Changed Successfully", TOAST_CONFIG);
        setIsLoading(false);
        refreshLedgerList();
      })
      .catch((error) => {
        setIsLoading(false);
        toast.error(error.message, TOAST_CONFIG);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });

    closeMenu();
  };

  const refreshLedgerList = () => {
    fetchLedgers();
  };

  const showLedgerBalance = (event) => {
    event.preventDefault();

    const ledgerIndex = currentLedgerElement.name;
    if (!ledgerIndex) {
      toast.error("Unable to Get Account", TOAST_CONFIG);
      return;
    }
    const balance = ledgersList[ledgerIndex]?.balance;

    if (balance == null) {
      toast.error("Unable to Get Balance", TOAST_CONFIG);
      return;
    }
    closeMenu();
  };

  const handlePageChange = (event, page) => {
    setCurrentPage(page);
    fetchLedgers(page);
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>Ledger Accounts</Title>
            <Box sx={{ display: "flex", ml: "auto", mt: -5, mb: 2 }}>
              <Button
                variant="contained"
                startIcon={<RefreshIcon />}
                sx={{ ml: 1 }}
                onClick={refreshLedgerList}
              >
                Refresh
              </Button>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                sx={{ ml: 1 }}
                onClick={toggleCreateModal}
              >
                Add Ledger
              </Button>
            </Box>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell>Category</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell align="right">Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {ledgersList.map(
                  (
                    {
                      id,
                      accountName,
                      accountDescription,
                      accountCategory,
                      accountStatus,
                    },
                    index
                  ) => {
                    const disabledText =
                      accountStatus !== STATUS.ACTIVE
                        ? { color: "#575757" }
                        : {};
                    const disabledRow =
                      accountStatus !== STATUS.ACTIVE
                        ? { backgroundColor: "#c9c9c9" }
                        : {};

                    return (
                      <TableRow
                        key={`${id}_${accountName}`}
                        style={disabledRow}
                      >
                        <TableCell style={disabledText}>
                          {capitalize(accountName)}
                        </TableCell>
                        <TableCell style={disabledText}>
                          {accountDescription}
                        </TableCell>
                        <TableCell style={disabledText}>
                          {accountCategory}
                        </TableCell>
                        <TableCell style={disabledText}>
                          {accountStatus}
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
              anchorEl={currentLedgerElement}
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
              {currentLedgerElement !== null &&
              currentLedgerElement.name !== undefined
                ? ledgersList[currentLedgerElement.name].accountStatus ==
                  STATUS.ACTIVE
                  ? [
                      <MenuItem
                        onClick={showLedgerAccountInfo}
                        key={`${currentLedgerElement.name}_view_enabled`}
                      >
                        View Ledger
                      </MenuItem>,
                      <MenuItem
                        onClick={showLedgerBalance}
                        key={`${currentLedgerElement.name}_reset`}
                      >
                        View Balance
                      </MenuItem>,
                      <MenuItem
                        onClick={showUpdateModal}
                        key={`${currentLedgerElement.name}_update_enabled`}
                      >
                        Update Ledger
                      </MenuItem>,
                      <MenuItem
                        onClick={toggleLedgerStatus}
                        key={`${currentLedgerElement.name}_disable`}
                      >
                        Deactivate Ledger
                      </MenuItem>,
                    ]
                  : [
                      <MenuItem
                        onClick={showLedgerAccountInfo}
                        key={`${currentLedgerElement.name}_view_disabled`}
                      >
                        View Ledger
                      </MenuItem>,
                      <MenuItem
                        onClick={toggleLedgerStatus}
                        key={`${currentLedgerElement.name}_enable`}
                      >
                        Activate Ledger
                      </MenuItem>,
                    ]
                : ""}
            </Menu>

            <LedgerCreateModal
              toggleModal={toggleCreateModal}
              modalOpen={createModalOpen}
              refreshLedgerList={refreshLedgerList}
            />
            <LedgerUpdateModal
              modalOpen={updateModalOpen}
              toggleModal={toggleUpdateModal}
              ledger={currentUpdateLedger}
              refreshLedgerList={refreshLedgerList}
            />
            <LedgerDetailsModal
              modalOpen={detailsModalOpen}
              toggleModal={toggleDetailsModal}
              ledger={currentLedgerDetails}
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
              count={noOfPages}
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
