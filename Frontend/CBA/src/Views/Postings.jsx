import { useState, useEffect } from "react";
import Container from "@mui/material/Container";
import Paper from "@mui/material/Paper";
import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import MenuItem from "@mui/material/MenuItem";
import Typography from "@mui/material/Typography";
import DialogTitle from "@mui/material/DialogTitle";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import Divider from "@mui/material/Divider";
import TextField from "@mui/material/TextField";
import InputLabel from "@mui/material/InputLabel";
import FormControl from "@mui/material/FormControl";
import Select from "@mui/material/Select";
import Backdrop from "@mui/material/Backdrop";
import Box from "@mui/material/Box";
import CircularProgress from "@mui/material/CircularProgress";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import dayjs from "dayjs";

import {
  TOAST_CONFIG,
  ROLES,
  STATUS,
  GENDER,
  POSTING_TYPES,
} from "../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../utils/validators";
import { toast } from "react-toastify";
import { updateUser } from "../api/users";
import {
  generateAccountStatement,
  getCustomerTransactions,
} from "../api/customer";
import Title from "../Components/Title";
import {
  createTransfer,
  createWithdrawal,
  createDeposit,
} from "../api/postings";

export default function Postings() {
  const [postingDetails, setPostingDetails] = useState({
    postingType: POSTING_TYPES.DEPOSIT,
    accountNumber: "",
    accountName: "",
    amount: 0,
    narration: "",
    receiverAccountName: "",
    receiverAccountNumber: "",
  });

  const [startDate, setStartDate] = useState(dayjs().subtract(1, "day"));
  const [endDate, setEndDate] = useState(dayjs());

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  const getEmptyFields = () => {
    const formFields = Object.entries(postingDetails);

    let emptyFields = {};

    for (const [key, value] of formFields) {
      if (!value) {
        if (postingDetails.postingType != POSTING_TYPES.TRANSFER)
          if (key == "receiverAccountName" || key == "receiverAccountNumber")
            continue;
        emptyFields[key] = "Field Cannot Be Empty";
      }
    }

    console.log({ emptyFields });
    return emptyFields;
  };

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    // setAccountNumber(String(value));
    setPostingDetails({ ...postingDetails, [name]: String(value) });
  };

  const handleSubmit = (event) => {
    console.log({ postingDetails });
    event.preventDefault();
    setIsLoading(true);
    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (Object.keys(emptyFields).length > 0) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    if (postingDetails.postingType == POSTING_TYPES.DEPOSIT) {
      const depositPosting = {
        ...postingDetails,
        datePosted: dayjs().format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
        customerNarration: postingDetails.narration,
        customerTransactionType: postingDetails.transactionType,
        customerAccountNumber: postingDetails.accountNumber,
      };
      submitDepositPosting(depositPosting);
    } else if (postingDetails.postingType == POSTING_TYPES.WITHDRAWAL) {
      const withdrawalPosting = {
        ...postingDetails,
        datePosted: dayjs().format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
        customerNarration: postingDetails.narration,
        customerTransactionType: postingDetails.transactionType,
        customerAccountNumber: postingDetails.accountNumber,
      };
      submitWithdrawalPosting(withdrawalPosting);
    } else if (postingDetails.postingType == POSTING_TYPES.TRANSFER) {
      const transferPosting = {
        ...postingDetails,
        datePosted: dayjs().format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
        senderAccountNumber: postingDetails.accountNumber,
      };
      submitTransferPosting(transferPosting);
    }
  };

  const submitTransferPosting = (transferPosting) => {
    createTransfer(transferPosting)
      .then((data) => {
        console.log(data);
        if (data.errors) throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const submitWithdrawalPosting = (withdrawalPosting) => {
    createWithdrawal(withdrawalPosting)
      .then((data) => {
        console.log(data);
        if (data.errors) throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const submitDepositPosting = (depositPosting) => {
    createDeposit(depositPosting)
      .then((data) => {
        console.log({ depositData: data });
        if (data.errors) throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const validateField = (event) => {
    const { name, value } = event.target;
    if (!value)
      setFormErrors({ ...formErrors, [name]: "Field Cannot Be Empty" });
    else if (formErrors[name]) {
      let updatedErrors = { ...formErrors };
      delete updatedErrors[name];
      setFormErrors({ ...updatedErrors });
    }
  };

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Paper sx={{ p: 2, display: "flex", flexDirection: "column" }}>
            <Title>Create Posting</Title>
            <Box
              component="form"
              onSubmit={handleSubmit}
              noValidate
              sx={{ mt: 1 }}
            >
              <Grid
                container
                rowSpacing={1}
                columnSpacing={{ xs: 1, sm: 2, md: 3 }}
                sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
              >
                <Grid xs={6} item>
                  <FormControl fullWidth sx={{ my: 2 }}>
                    <InputLabel id="PostingType-label">Posting Type</InputLabel>
                    <Select
                      labelId="PostingType-label"
                      id="PostingType"
                      value={
                        postingDetails.postingType || POSTING_TYPES.DEPOSIT
                      }
                      label="PostingType"
                      name="postingType"
                      onChange={handleInputChange}
                    >
                      {Object.values(POSTING_TYPES).map(
                        (postingType, index) => {
                          const postingTypeKey = `update_${postingType}_${index}`;
                          return (
                            <MenuItem value={postingType} key={postingTypeKey}>
                              {postingType}
                            </MenuItem>
                          );
                        }
                      )}
                    </Select>
                  </FormControl>
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="AccountNumber"
                    label="AccountNumber"
                    name="accountNumber"
                    onChange={handleInputChange}
                    error={Boolean(formErrors.accountNumber)}
                    helperText={formErrors.accountNumber}
                    onBlur={validateField}
                    value={postingDetails.accountNumber}
                  />
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="AccountName"
                    label="AccountName"
                    name="accountName"
                    onChange={handleInputChange}
                    error={Boolean(formErrors.accountName)}
                    helperText={formErrors.accountName}
                    onBlur={validateField}
                    value={postingDetails.accountName}
                  />
                </Grid>
                <Grid item xs={6}>
                  <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="Narration"
                    label="Narration"
                    name="narration"
                    onChange={handleInputChange}
                    error={Boolean(formErrors.narration)}
                    helperText={formErrors.narration}
                    onBlur={validateField}
                    value={postingDetails.narration}
                  />
                </Grid>
                {postingDetails.postingType == POSTING_TYPES.TRANSFER && (
                  <>
                    <Grid item xs={6}>
                      <TextField
                        margin="normal"
                        required
                        fullWidth
                        id="ReceiverAccountNumber"
                        label="ReceiverAccountNumber"
                        name="receiverAccountNumber"
                        onChange={handleInputChange}
                        error={Boolean(formErrors.receiverAccountNumber)}
                        helperText={formErrors.receiverAccountNumber}
                        onBlur={validateField}
                        value={postingDetails.receiverAccountNumber}
                      />
                    </Grid>
                    <Grid item xs={6}>
                      <TextField
                        margin="normal"
                        required
                        fullWidth
                        id="ReceiverAccountName"
                        label="ReceiverAccountName"
                        name="receiverAccountName"
                        onChange={handleInputChange}
                        error={Boolean(formErrors.receiverAccountName)}
                        helperText={formErrors.receiverAccountName}
                        onBlur={validateField}
                        value={postingDetails.receiverAccountName}
                      />
                    </Grid>
                  </>
                )}
                <Grid item xs={12}>
                  <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="Amount"
                    label="Amount"
                    name="amount"
                    onChange={handleInputChange}
                    error={Boolean(formErrors.amount)}
                    helperText={formErrors.amount}
                    onBlur={validateField}
                    value={postingDetails.amount}
                    inputProps={{ inputMode: "numeric" }}
                  />
                </Grid>
                <Grid item xs={12}>
                  <Button
                    type="submit"
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                    fullWidth
                  >
                    Submit
                  </Button>
                </Grid>
              </Grid>
            </Box>
            <Backdrop
              sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
              open={isLoading}
            >
              <CircularProgress color="inherit" />
            </Backdrop>
          </Paper>
        </Grid>
      </Grid>
    </Container>
  );
}
