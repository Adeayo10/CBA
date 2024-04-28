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

import Title from "../../../Components/Title";

import PostingCreateModal from "./PostingCreateModal";

import {
  TOAST_CONFIG,
  STATUS,
  ACCOUNT_IDS,
  PAGE_SIZE,
  CREATE_ACCOUNT_BASE,
} from "../../../utils/constants";

import { toast } from "react-toastify";

import { forgotPassword } from "../../../api/auth";
import { getUsers, deactivateUser, activateUser } from "../../../api/users";
import {
  changeCustomerAccountStatus,
  getCustomers,
} from "../../../api/customer";
import {
  capitalize,
  extractUpdateFields,
  formatCurrency,
  formatDate,
} from "../../../utils/util";
import { ErrorTwoTone } from "@mui/icons-material";
import { redirectIfRefreshTokenExpired } from "../../../utils/token";

import { getPostings } from "../../../api/postings";

export default function Postings({ postingType }) {
  const [postingsList, setPostingsList] = useState([]);
  const [currentPostingsElement, setCurrentPostingsElement] = useState(null);

  const [currentPostingDetails, setCurrentPostingDetails] = useState({});

  const [detailsModalOpen, setDetailsModalOpen] = useState(false);
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const [currentPage, setCurrentPage] = useState(1);
  const [noOfPages, setNoOfPages] = useState(1);

  const menuOpen = Boolean(currentPostingsElement);
  const navigate = useNavigate();

  useEffect(() => {
    fetchPostings();
    return () => {
      setPostingsList([]);
    };
  }, [postingType]);

  const fetchPostings = async (pageNumber = currentPage) => {
    setIsLoading(true);
    getPostings(postingType, pageNumber)
      .then((data) => {
        console.log(data);
        if (!data.filteredPostings)
          throw new Error(data.message || data.errors);

        setPostingsList(data.filteredPostings);
        setNoOfPages(
          Math.ceil(data.totalPostingsByType[postingType] / PAGE_SIZE)
        );
        setIsLoading(false);
      })
      .catch((error) => {
       const errorMessage = error.message || "No Data Found"
        toast.error(errorMessage, TOAST_CONFIG);
        setIsLoading(false);
        redirectIfRefreshTokenExpired(error.message, navigate);
      });
  };

  const openMenu = (event) => {
    setCurrentPostingsElement(event.currentTarget);
  };

  const closeMenu = () => {
    setCurrentPostingsElement(null);
  };

  const showPostingInfo = (event) => {
    event.preventDefault();
    const postingIndex = currentPostingsElement.name;
    const posting = postingsList[postingIndex];
    setCurrentPostingDetails(posting);
    toggleDetailsModal();
    closeMenu();
  };

  const toggleDetailsModal = () => {
    setDetailsModalOpen(!detailsModalOpen);
  };

  const toggleCreateModal = () => {
    setCreateModalOpen(!createModalOpen);
  };

  const refreshPostingsList = () => {
    fetchPostings();
  };

  const handlePageChange = (event, page) => {
    setCurrentPage(page);
    fetchPostings(page);
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>{postingType} Postings</Title>
            <Box sx={{ display: "flex", ml: "auto", mt: -5, mb: 2 }}>
              <Button
                variant="contained"
                startIcon={<RefreshIcon />}
                sx={{ ml: 1 }}
                onClick={refreshPostingsList}
              >
                Refresh
              </Button>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                sx={{ ml: 1 }}
                onClick={toggleCreateModal}
              >
                Make {postingType}
              </Button>
            </Box>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Account Name</TableCell>
                  <TableCell>Account No.</TableCell>
                  <TableCell>Account Type</TableCell>
                  <TableCell>Branch</TableCell>
                  <TableCell>Teller</TableCell>
                  <TableCell>Amount</TableCell>
                  <TableCell>Date</TableCell>
                  <TableCell align="right">Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {postingsList.map(
                  (
                    {
                      id,
                      accountName,
                      accountNumber,
                      accountType,
                      branch,
                      teller,
                      amount,
                      datePosted,
                    },
                    index
                  ) => {
                    return (
                      <TableRow key={id}>
                        <TableCell>{accountName}</TableCell>
                        <TableCell>{lastName}</TableCell>
                        <TableCell>{accountNumber}</TableCell>
                        <TableCell>{accountType}</TableCell>
                        <TableCell>{branch}</TableCell>
                        <TableCell>{teller}</TableCell>
                        <TableCell>{formatCurrency(amount)}</TableCell>
                        <TableCell>{formatDate(datePosted)}</TableCell>
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
              anchorEl={currentPostingsElement}
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
              {currentPostingsElement !== null &&
              currentPostingsElement.name !== undefined
                ? [
                    <MenuItem
                      onClick={showPostingInfo}
                      key={`${currentPostingsElement.name}_view_enabled`}
                    >
                      View Posting
                    </MenuItem>,
                  ]
                : ""}
            </Menu>

            <PostingCreateModal
              toggleModal={toggleCreateModal}
              modalOpen={createModalOpen}
              postingType={postingType}
              refreshPostingsList={refreshPostingsList}
            />
            {/* <AccountDetailsModal
              modalOpen={detailsModalOpen}
              toggleModal={toggleDetailsModal}
              account={currentPostingDetails}
              accountType={postingType}
            /> */}
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
