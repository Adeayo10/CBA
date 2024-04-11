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

import { TOAST_CONFIG, ROLES } from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
import { updateUser } from "../../../api/users";

export default function UserUpdateModal({
  toggleModal,
  modalOpen,
  user,
  userBranch,
  refreshUsersList,
}) {
  if (!user.id || !userBranch.userId) return <></>;

  const [userDetails, setUserDetails] = useState({});
  const [userBranchDetails, setUserBranchDetails] = useState({});
  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (modalOpen) {
      setUserDetails({ ...user });
      setUserBranchDetails({ ...userBranch });
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

    updateUser({ ...userDetails, bankBranch: { ...userBranchDetails } })
      .then((data) => {
        //console.log(data);
        if (!data.success || data.errors)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshUsersList();
        toggleModal();
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

  return (
    <Dialog
      open={modalOpen}
      onClose={toggleModal}
      keepMounted={false}
      key={`${user.id}_modal`}
      PaperProps={{
        component: "form",
        onSubmit: handleSubmit,
        noValidate: true,
      }}
    >
      <DialogTitle>Update User</DialogTitle>
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
              id="Username"
              label="Username"
              name="userName"
              value={userDetails.userName}
              disabled
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
              value={userDetails.email}
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
              onChange={handleUserChange}
              error={Boolean(formErrors.fullName)}
              helperText={formErrors.fullName}
              onBlur={validateField}
              value={userDetails.fullName}
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
              onChange={handleUserChange}
              error={Boolean(formErrors.address)}
              helperText={formErrors.address}
              onBlur={validateField}
              value={userDetails.address}
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
              value={userDetails.phoneNumber}
            />
          </Grid>
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="Role-label">Role</InputLabel>
              <Select
                labelId="Role-label"
                id="Role"
                value={userDetails.role || ROLES.USER}
                label="Role"
                name="role"
                onChange={handleUserChange}
              >
                {Object.values(ROLES).map((role, index) => {
                  const roleKey = `update_${role}`;
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
              id="Code"
              label="Code"
              name="code"
              disabled
              value={userBranchDetails.code}
            />
          </Grid>
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
              value={userBranchDetails.name}
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
              value={userBranchDetails.region}
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
              value={userBranchDetails.description}
            />
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
