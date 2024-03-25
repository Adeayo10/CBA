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

import { TOAST_CONFIG, ROLES, STATUS, GENDER } from "../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../utils/validators";
import { toast } from "react-toastify";
import { updateUser } from "../api/users";
import { updateCustomer } from "../api/customer";

export default function AccountUpdateModal({
  toggleModal,
  modalOpen,
  account,
  accountType,
  refreshAccountsList,
}) {
  if (!account.id) return <></>;

  const [accountDetails, setAccountDetails] = useState({});
  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (modalOpen) {
      setAccountDetails({ ...account, accountType });
      //console.log("I ran")
    }
  }, [modalOpen]);

  const handleSubmit = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (Object.keys(emptyFields).length > 0) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    updateCustomer(accountDetails)
      .then((data) => {
        //console.log(data);
        if (data.errors) throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshAccountsList();
        toggleModal();
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

  return (
    <Dialog
      open={modalOpen}
      onClose={toggleModal}
      keepMounted={false}
      key={`${account.id}_modal`}
      PaperProps={{
        component: "form",
        onSubmit: handleSubmit,
        noValidate: true,
      }}
    >
      <DialogTitle>Update Account</DialogTitle>
      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Typography gutterBottom variant="h6">
          {accountType} Account: {account.accountNumber}
        </Typography>
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
              id="Email"
              label="Email"
              name="email"
              value={accountDetails.email}
              disabled
            />
          </Grid>
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
              value={accountDetails.fullName}
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
              value={accountDetails.address}
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
              value={accountDetails.phoneNumber}
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
                value={accountDetails.state || STATUS.ACTIVE}
                label="State"
                name="state"
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
        <Button onClick={toggleModal}>Cancel</Button>
        <Button type="submit">Update</Button>
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
