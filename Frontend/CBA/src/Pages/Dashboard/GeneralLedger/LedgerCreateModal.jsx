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
  LEDGER_TYPES,
  CREATE_LEDGER_BASE,
} from "../../../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../../../utils/validators";
import { toast } from "react-toastify";
// import { generateAccountNumber, generateId } from "../../../utils/util";
import { createCustomer } from "../../../api/customer";
import { createLedger } from "../../../api/ledger";

function generateId(){
  return 10
}

export default function LedgerCreateModal({
  toggleModal,
  modalOpen,
  refreshAccountsList,
}) {
  const ledgerId = generateId();

  const [ledgerDetails, setLedgerDetails] = useState({
    ...CREATE_LEDGER_BASE,
    id: ledgerId,
  });

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    setLedgerDetails({
      ...CREATE_LEDGER_BASE,
      id: generateId(),
    });
  }, []);

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

    createLedger(ledgerDetails)
      .then((data) => {
        //console.log(data);
        if (data.errors || !data.status)
          throw new Error(data.message || data.errors);

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

  const resetModal = () => {
    setLedgerDetails({
      ...CREATE_LEDGER_BASE,
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
      <DialogTitle>Create Ledger Account</DialogTitle>
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
              id="AccountName"
              label="Account Name"
              name="accountName"
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
              id="AccountDescription"
              label="Account Description"
              name="accountDescription"
              onChange={handleInputChange}
              error={Boolean(formErrors.email)}
              helperText={formErrors.email}
              onBlur={validateField}
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
