import { useState, useEffect } from "react";
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
import CircularProgress from "@mui/material/CircularProgress";

import {
  TOAST_CONFIG,
  POSTING_TYPES,
  CREATE_POSTING_BASE,
  POSTING_AUTOGEN_FIELD_MAP,
} from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
import { getCurrentISODate, generateId } from "../../../utils/util";
import { createCustomer, getCustomerByAccount } from "../../../api/customer";

import {
  createTransfer,
  createWithdrawal,
  createDeposit,
} from "../../../api/postings";

export default function PostingCreateModal({
  toggleModal,
  modalOpen,
  postingType,
  refreshPostingsList,
}) {
  const postingId = generateId();

  const [postingDetails, setPostingDetails] = useState({
    ...CREATE_POSTING_BASE[postingType],
    id: postingId,
  });

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setPostingDetails({
      ...CREATE_POSTING_BASE[postingType],
      id: generateId(),
    });
  }, [postingType]);

  const getEmptyFields = () => {
    const formFields = Object.entries(postingDetails);

    let emptyFields = {};

    for (const [key, value] of formFields) {
      if (!value) {
        if (Object.keys(POSTING_AUTOGEN_FIELD_MAP).includes(key)) continue;
        if (key == "customerNarration" || key == "datePosted") continue;
        emptyFields[key] = "Field Cannot Be Empty";
      }
    }

    console.log({ emptyFields });
    return emptyFields;
  };

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setPostingDetails({ ...postingDetails, [name]: String(value) });
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setIsLoading(true);
    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (
      Object.keys(emptyFields).length > 0 ||
      Object.keys(formErrors).length > 0
    ) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    try {
      let createPosting = null;
      switch (postingType) {
        case POSTING_TYPES.DEPOSIT:
          createPosting = submitDepositPosting;
          break;
        case POSTING_TYPES.WITHDRAWAL:
          createPosting = submitWithdrawalPosting;
          break;
        case POSTING_TYPES.TRANSFER:
          createPosting = submitTransferPosting;
          break;
        default:
          createPosting = null;
          break;
      }

      console.log(createPosting);

      if (createPosting == null)
        throw new Error("Invalid Posting Type Detected");
      const { errors, message, status } = await createPosting();

      if (!status || errors) throw new Error(message || errors);
      toast.success(message, TOAST_CONFIG);
      refreshPostingsList();
      resetModal();
    } catch (error) {
      console.log(error);
      toast.error(error.message, TOAST_CONFIG);
    }

    setIsLoading(false);
  };

  const customerLookup = async (accountNumber) => {
    console.log(accountNumber);
    const { status, customer } = await getCustomerByAccount(accountNumber);
    if (!status)
      throw new Error(
        `Failed to get Customer with Account Number: ${accountNumber}`
      );
    return customer;
  };

  const getAutogenFields = async (accountNumber) => {
    const customer = await customerLookup(accountNumber);

    const autogenFields = {};

    for (const [mappedField, field] of Object.entries(
      POSTING_AUTOGEN_FIELD_MAP
    )) {
      autogenFields[mappedField] = customer[field].toString();
    }

    return autogenFields;
  };

  const submitDepositPosting = async () => {
    const autogenFields = await getAutogenFields(
      postingDetails.customerAccountNumber
    );
    const depositPosting = {
      ...postingDetails,
      datePosted: getCurrentISODate(),
      customerNarration: postingDetails.narration,
      customerDeposit: postingDetails.amount,
      ...autogenFields,
    };

    console.log({ depositPosting });
    return await createDeposit(depositPosting);
  };

  const submitWithdrawalPosting = async () => {
    const autogenFields = await getAutogenFields(
      postingDetails.customerAccountNumber
    );
    const withdrawalPosting = {
      ...postingDetails,
      datePosted: getCurrentISODate(),
      customerNarration: postingDetails.narration,
      customerWithdrawal: postingDetails.amount,
      ...autogenFields,
    };
    return await createWithdrawal(withdrawalPosting);
  };

  const submitTransferPosting = async () => {
    const sender = await customerLookup(postingDetails.senderAccountNumber);
    const reciever = await customerLookup(postingDetails.receiverAccountNumber);
    const transferPosting = {
      ...postingDetails,
      datePosted: getCurrentISODate(),
      receiverAccountName: reciever.fullName,
      recieverId: reciever.id,
      senderId: sender.id,
    };
    return await createTransfer(transferPosting);
  };

  const validateField = (event) => {
    const { name, value } = event.target;
    if (!value)
      setFormErrors({ ...formErrors, [name]: "Field Cannot Be Empty" });
    else if (name === "phoneNumber" && !isValidPhoneNumber(value))
      setFormErrors({ ...formErrors, [name]: "Invalid Phone Number" });
    else if (name === "email" && !isValidEmail(value))
      setFormErrors({ ...formErrors, [name]: "Invalid Email Address" });
    else if (formErrors[name]) {
      let updatedErrors = { ...formErrors };
      delete updatedErrors[name];
      setFormErrors({ ...updatedErrors });
    }
  };

  const resetModal = () => {
    setPostingDetails({
      ...CREATE_POSTING_BASE[postingType],
      id: generateId(),
    });
    setFormErrors({});
    toggleModal();
  };

  return (
    <Dialog
      open={modalOpen}
      onClose={resetModal}
      PaperProps={{
        component: "form",
        onSubmit: handleSubmit,
        noValidate: true,
      }}
    >
      <DialogTitle>Create {postingType} Posting</DialogTitle>
      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Grid
          container
          rowSpacing={1}
          columnSpacing={{ xs: 1, sm: 2, md: 3 }}
          sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
        >
          {postingType !== POSTING_TYPES.TRANSFER && (
            <>
              <Grid item xs={6}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="LedgerAccountNumber"
                  label="LedgerAccountNumber"
                  name="ledgerAccountNumber"
                  onChange={handleInputChange}
                  error={Boolean(formErrors.ledgerAccountNumber)}
                  helperText={formErrors.ledgerAccountNumber}
                  onBlur={validateField}
                  value={postingDetails.ledgerAccountNumber}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="LedgerAccountName"
                  label="LedgerAccountName"
                  name="ledgerAccountName"
                  onChange={handleInputChange}
                  error={Boolean(formErrors.ledgerAccountName)}
                  helperText={formErrors.ledgerAccountName}
                  onBlur={validateField}
                  value={postingDetails.ledgerAccountName}
                />
              </Grid>
              <Grid item xs={6}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="CustomerAccountNumber"
                  label="CustomerAccountNumber"
                  name="customerAccountNumber"
                  onChange={handleInputChange}
                  error={Boolean(formErrors.customerAccountNumber)}
                  helperText={formErrors.customerAccountNumber}
                  onBlur={validateField}
                  value={postingDetails.customerAccountNumber}
                />
              </Grid>
            </>
          )}

          <Grid item xs={6}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="CustomerAccountName"
              label="CustomerAccountName"
              name="customerAccountName"
              onChange={handleInputChange}
              error={Boolean(formErrors.customerAccountName)}
              helperText={formErrors.customerAccountName}
              onBlur={validateField}
              value={postingDetails.customerAccountName}
            />
          </Grid>

          {postingType == POSTING_TYPES.TRANSFER && (
            <>
              <Grid item xs={6}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="CustomerAccountNumber"
                  label="CustomerAccountNumber"
                  name="senderAccountNumber"
                  onChange={handleInputChange}
                  error={Boolean(formErrors.senderAccountNumber)}
                  helperText={formErrors.senderAccountNumber}
                  onBlur={validateField}
                  value={postingDetails.senderAccountNumber}
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
            </>
          )}
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
          <Grid item xs={6}>
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
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={resetModal}>Cancel</Button>
        <Button type="submit">Create</Button>
      </DialogActions>
      <Backdrop
        sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={isLoading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </Dialog>
  );
}
