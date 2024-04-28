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
  CREATE_ACCOUNT_BASE,
  GENDER,
  STATUS,
  NG_STATES,
} from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
import { generateAccountNumber, generateId } from "../../../utils/util";
import { createCustomer } from "../../../api/customer";

export default function AccountCreateModal({
  toggleModal,
  modalOpen,
  accountType,
  refreshAccountsList,
}) {
  const customerId = generateId();

  const [accountDetails, setAccountDetails] = useState({
    ...CREATE_ACCOUNT_BASE,
    id: customerId,
    accountType,
    accountNumber: generateAccountNumber(),
  });

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setAccountDetails({
      ...CREATE_ACCOUNT_BASE,
      id: generateId(),
      accountType,
      accountNumber: generateAccountNumber(),
    });
  }, [accountType]);

  const handleSubmit = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (Object.keys(emptyFields).length > 0 || Object.keys(formErrors).length > 0) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    createCustomer(accountDetails)
      .then((data) => {
        if (data.errors) throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshAccountsList();
        resetModal();
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const getEmptyFields = () => {
    const formFields = Object.entries(accountDetails);
    let emptyFields = {};

    for (const [key, value] of formFields) {
      if (!value) {
        emptyFields[key] = "Field Cannot Be Empty";
      }
    }

    console.log({ emptyFields });
    return emptyFields;
  };

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setAccountDetails({ ...accountDetails, [name]: String(value) });
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
    setAccountDetails({
      ...CREATE_ACCOUNT_BASE,
      id: generateId(),
      accountType,
      accountNumber: generateAccountNumber(),
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
      <DialogTitle>Create {accountType} Account</DialogTitle>
      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Grid
          container
          rowSpacing={1}
          columnSpacing={{ xs: 1, sm: 2, md: 3 }}
          sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
        >
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Fullname"
              label="Fullname"
              name="fullName"
              onChange={handleInputChange}
              error={Boolean(formErrors.fullName)}
              helperText={formErrors.fullName}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Email"
              label="Email"
              name="email"
              onChange={handleInputChange}
              error={Boolean(formErrors.email)}
              helperText={formErrors.email}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Address"
              label="Address"
              name="address"
              onChange={handleInputChange}
              error={Boolean(formErrors.address)}
              helperText={formErrors.address}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Phonenumber"
              label="Phone Number"
              name="phoneNumber"
              onChange={handleInputChange}
              error={Boolean(formErrors.phoneNumber)}
              helperText={formErrors.phoneNumber}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Branch"
              label="Branch"
              name="branch"
              value={accountDetails.branch}
              onChange={handleInputChange}
              error={Boolean(formErrors.branch)}
              helperText={formErrors.branch}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="Gender-label">Gender</InputLabel>
              <Select
                labelId="Gender-label"
                id="Gender"
                value={accountDetails.gender || GENDER.MALE}
                label="Gender"
                name="gender"
                onChange={handleInputChange}
              >
                {Object.values(GENDER).map((gender, index) => {
                  const genderKey = `update_${gender}_${index}`;
                  return (
                    <MenuItem value={gender} key={genderKey}>
                      {gender}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
          </Grid>
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="State-label">State</InputLabel>
              <Select
                labelId="State-label"
                id="State"
                value={accountDetails.state}
                label="State"
                name="state"
                onChange={handleInputChange}
              >
                {NG_STATES.map((state, index) => {
                  const stateKey = `update_${state}_${index}`;
                  return (
                    <MenuItem value={state} key={stateKey}>
                      {state}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
          </Grid>
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="Status-label">Status</InputLabel>
              <Select
                labelId="Status-label"
                id="Status"
                value={accountDetails.status}
                label="Status"
                name="status"
                onChange={handleInputChange}
              >
                {Object.values(STATUS).map((status, index) => {
                  const statusKey = `update_${status}_${index}`;
                  return (
                    <MenuItem value={status} key={statusKey}>
                      {status}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
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
