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

import { TOAST_CONFIG, ROLES, STATUS, GENDER } from "../utils/constants";
import { isValidEmail, isValidPhoneNumber } from "../utils/validators";
import { toast } from "react-toastify";
import { updateUser } from "../api/users";
import {
  generateAccountStatement,
  getCustomerTransactions,
} from "../api/customer";
import Title from "../Components/Title";

export default function AccountStatement() {
  const [accountNumber, setAccountNumber] = useState("");

  const [startDate, setStartDate] = useState(dayjs().subtract(1, "day"));
  const [endDate, setEndDate] = useState(dayjs());

  const [formErrors, setFormErrors] = useState({});

  const [isLoading, setIsLoading] = useState(false);

  // useEffect(() => {
  //   setAccountStatementDetails({
  //     ...CREATE_ACCOUNT_BASE,
  //     id: generateId(),
  //     accountType,
  //     accountNumber: generateAccountNumber(),
  //   });
  // }, [accountType]);

  const exportStatement = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (Object.keys(emptyFields).length > 0) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    const accountStatementDetails = {
      startDate: dayjs(startDate).format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
      endDate: dayjs(endDate).format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
      accountNumber,
    };

    generateAccountStatement(accountStatementDetails)
      .then((data) => {
        console.log(data);
        if (data.errors || !data.status)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const getStatement = (event) => {
    event.preventDefault();
    setIsLoading(true);

    const emptyFields = getEmptyFields();

    setFormErrors({ ...emptyFields });

    if (Object.keys(emptyFields).length > 0) {
      setIsLoading(false);
      toast.error("Form contains errors", TOAST_CONFIG);
      return;
    }

    console.log({ endDate: dayjs(endDate).format(`YYYY-MM-DDTHH:mm:ss.SSSZ`) });

    const accountStatementDetails = {
      startDate: dayjs(startDate).format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
      endDate: dayjs(endDate).format(`YYYY-MM-DDTHH:mm:ss.SSSZ`),
      accountNumber,
    };

    getCustomerTransactions(accountStatementDetails)
      .then((data) => {
        console.log({ data });
        if (data.errors || !data.status)
          throw new Error(data.message || data.errors);

        toast.success(data.message, TOAST_CONFIG);
        setIsLoading(false);
      })
      .catch((error) => {
        toast.error(error.message, TOAST_CONFIG);
        setIsLoading(false);
      });
  };

  const getEmptyFields = () => {
    let emptyFields = {};

    if (!startDate) {
      emptyFields["startDate"] = "Field Cannot Be Empty";
    }
    if (!endDate) {
      emptyFields["endDate"] = "Field Cannot Be Empty";
    }
    if (!accountNumber) {
      emptyFields["endDate"] = "Field Cannot Be Empty";
    }

    console.log({ emptyFields });
    return emptyFields;
  };

  const handleInputChange = (event) => {
    const { value } = event.target;
    setAccountNumber(String(value));
  };

  const handleStartDateChange = (dateValue) => {
    console.log({ dateValue });
    setStartDate(dateValue);
  };

  const handleEndDateChange = (dateValue) => {
    console.log({ dateValue });
    setEndDate(dateValue);
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
            <Title>Generate Account Statements</Title>
            <Box noValidate sx={{ mt: 1 }}>
              <Grid item>
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
                  value={accountNumber}
                />
              </Grid>
              <Grid
                container
                rowSpacing={1}
                columnSpacing={{ xs: 1, sm: 2, md: 3 }}
                sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
              >
                <Grid item>
                  <DatePicker
                    label="Start Date"
                    name="startDate"
                    onChange={handleStartDateChange}
                    value={startDate}
                    slotProps={{
                      textField: {
                        helperText: formErrors.startDate,
                        error: Boolean(formErrors.startDate),
                        onBlur: validateField,
                        required: true,
                      },
                    }}
                  />
                </Grid>
                <Grid item>
                  <DatePicker
                    label="End Date"
                    name="endDate"
                    onChange={handleEndDateChange}
                    value={endDate}
                    slotProps={{
                      textField: {
                        helperText: formErrors.endDate,
                        error: Boolean(formErrors.endDate),
                        onBlur: validateField,
                        required: true,
                      },
                    }}
                  />
                </Grid>
              </Grid>

              <Grid
                container
                rowSpacing={1}
                columnSpacing={{ xs: 1, sm: 2, md: 3 }}
                sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
              >
                <Grid item xs={6}>
                  <Button
                    type="button"
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                    fullWidth
                    onClick={getStatement}
                  >
                    View Statement
                  </Button>
                </Grid>
                <Grid item xs={6}>
                  <Button
                    type="button"
                    variant="contained"
                    sx={{ mt: 3, mb: 2 }}
                    fullWidth
                    onClick={exportStatement}
                  >
                    Export Statement
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
