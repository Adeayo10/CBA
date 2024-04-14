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

import { TOAST_CONFIG, LEDGER_TYPES } from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
import { updateUser } from "../../../api/users";
import { updateCustomer } from "../../../api/customer";
import { updateLedger } from "../../../api/ledger";

export default function LedgerUpdateModal({
  toggleModal,
  modalOpen,
  ledger,
  refreshLedgerList,
}) {
  if (!ledger.accountName) return <></>;

  const [ledgerDetails, setLedgerDetails] = useState({ ...ledger });
  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (modalOpen) {
      setLedgerDetails({ ...ledger });
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

    updateLedger(ledgerDetails)
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.status)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
        refreshLedgerList();
        toggleModal();
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const getEmptyFields = () => {
    const formFields = Object.entries(ledgerDetails);

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
    setLedgerDetails({ ...ledgerDetails, [name]: String(value) });
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
    <Dialog
      open={modalOpen}
      onClose={toggleModal}
      keepMounted={false}
      key={`${ledger.id}_modal`}
      PaperProps={{
        component: "form",
        onSubmit: handleSubmit,
        noValidate: true,
      }}
    >
      <DialogTitle>Update Ledger Account</DialogTitle>
      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Typography gutterBottom variant="h6">
          Ledger Account: {ledger.accountName}
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
              id="AccountName"
              label="Account Name"
              name="accountName"
              onChange={handleInputChange}
              error={Boolean(formErrors.accountName)}
              helperText={formErrors.accountName}
              onBlur={validateField}
              value={ledgerDetails.accountName}
            />
          </Grid>
          <Grid xs={6} item>
            <TextField
              margin="normal"
              required
              fullWidth
              id="AccountDescription"
              label="Account Description"
              name="accountDescription"
              onChange={handleInputChange}
              error={Boolean(formErrors.accountDescription)}
              helperText={formErrors.accountDescription}
              onBlur={validateField}
              value={ledgerDetails.accountDescription}
            />
          </Grid>
          <Grid xs={6} item>
            <FormControl fullWidth sx={{ my: 2 }}>
              <InputLabel id="AccountCategory-label">
                Account Category
              </InputLabel>
              <Select
                labelId="AccountCategory-label"
                id="AccountCategory"
                value={ledgerDetails.accountCategory || LEDGER_TYPES.ASSET}
                label="Gender"
                name="accountCategory"
                onChange={handleInputChange}
              >
                {Object.values(LEDGER_TYPES).map((ledgerType, index) => {
                  const ledgerTypeKey = `update_${ledgerType}_${index}`;
                  return (
                    <MenuItem value={ledgerType} key={ledgerTypeKey}>
                      {ledgerType}
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
