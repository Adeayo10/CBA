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
  ROLES,
  CREATE_USER_BASE,
  CREATE_USER_BRANCH_BASE,
} from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
import { createUser } from "../../../api/users";
import { generateId, generateRandomPassword } from "../../../utils/util";

export default function UserCreateModal({
  toggleModal,
  modalOpen,
  refreshUsersList,
}) {
  const userId = generateId();

  const [userDetails, setUserDetails] = useState({
    ...CREATE_USER_BASE,
    id: userId,
    password: generateRandomPassword(),
  });
  const [userBranchDetails, setUserBranchDetails] = useState({
    ...CREATE_USER_BRANCH_BASE,
    userId: userId,
  });
  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

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

    createUser({ ...userDetails, bankBranch: { ...userBranchDetails } })
      .then((data) => {
        //console.log(data);
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshUsersList();
        resetModal();
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const getEmptyFields = () => {
    const formFields = Object.entries(userDetails).concat(
      Object.entries(userBranchDetails)
    );

    let emptyFields = {};

    for (const [key, value] of formFields) {
      if (!value) {
        emptyFields[key] = "Field Cannot Be Empty";
      }
    }

    return emptyFields;
  };

  const handleUserChange = (event) => {
    const { name, value } = event.target;
    setUserDetails({ ...userDetails, [name]: String(value) });
  };

  const handleUserBranchChange = (event) => {
    const { name, value } = event.target;
    setUserBranchDetails({ ...userBranchDetails, [name]: String(value) });
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
    const newId = generateId();
    const newPassword = generateRandomPassword();
    setUserDetails({ ...CREATE_USER_BASE, id: newId, password: newPassword });
    setUserBranchDetails({ ...CREATE_USER_BRANCH_BASE, userId: newId });
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
      <DialogTitle>Create User</DialogTitle>
      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Typography gutterBottom variant="h6">
          User
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
              id="Fullname"
              label="Fullname"
              name="fullName"
              onChange={handleUserChange}
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
              id="Username"
              label="Username"
              name="userName"
              onChange={handleUserChange}
              error={Boolean(formErrors.userName)}
              helperText={formErrors.userName}
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
              onChange={handleUserChange}
              error={Boolean(formErrors.email)}
              helperText={formErrors.email}
              onBlur={validateField}
            />
          </Grid>
          {/* <Grid xs={6} item>
            <TextField
              margin="normal"
              fullWidth
              id="Password"
              label="Password"
              name="password"
              onChange={handleUserChange}
              value={userDetails.password}
              disabled
            />
          </Grid> */}
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Address"
              label="Address"
              name="address"
              onChange={handleUserChange}
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
              onChange={handleUserChange}
              error={Boolean(formErrors.phoneNumber)}
              helperText={formErrors.phoneNumber}
              onBlur={validateField}
            />
          </Grid>
          {/* <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Status"
              label="Status"
              name="status"
              onChange={handleUserChange}
              error={Boolean(formErrors.status)}
              helperText={formErrors.status}
              onBlur={validateField}
            />
          </Grid> */}
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="Role-label">Role</InputLabel>
              <Select
                labelId="Role-label"
                id="Role"
                value={userDetails.role}
                label="Role"
                name="role"
                onChange={handleUserChange}
              >
                {Object.values(ROLES).map((role, index) => {
                  const roleKey = `${userId}_${role}`;
                  return (
                    <MenuItem value={role} key={roleKey}>
                      {role}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
          </Grid>
        </Grid>
        <Divider sx={{ my: 1, width: "100%" }} />
        <Typography gutterBottom variant="h6">
          Branch
        </Typography>
        <Grid
          container
          rowSpacing={1}
          columnSpacing={{ xs: 1, sm: 2, md: 3 }}
          sx={{ my: 0.5, py: 0.5 }}
        >
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Name"
              label="Name"
              name="name"
              onChange={handleUserBranchChange}
              error={Boolean(formErrors.name)}
              helperText={formErrors.name}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Region"
              label="Region"
              name="region"
              onChange={handleUserBranchChange}
              error={Boolean(formErrors.region)}
              helperText={formErrors.region}
              onBlur={validateField}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="Description"
              label="Description"
              name="description"
              onChange={handleUserBranchChange}
              error={Boolean(formErrors.description)}
              helperText={formErrors.description}
              onBlur={validateField}
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
