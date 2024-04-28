import { useState } from "react";
import {useNavigate } from "react-router-dom";
import Button from "@mui/material/Button";
import DialogTitle from "@mui/material/DialogTitle";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import Backdrop from "@mui/material/Backdrop";
import DialogContentText from '@mui/material/DialogContentText';
import CircularProgress from "@mui/material/CircularProgress";

import { clearTokenData } from "../utils/token";
import { logoutUser } from "../api/auth";

import { TOAST_CONFIG, ROUTES } from "../utils/constants";
import { toast } from "react-toastify";

export default function LogoutModal({ modalOpen, closeModal }) {

  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const logout = (event) => {
    event.preventDefault();
    setIsLoading(true);
    logoutUser()
      .then((data) => {
        if (!data.ok || data.error) throw new Error(data.message || data.error);

        clearTokenData();
        setIsLoading(false);
        toast.success("Successfull", TOAST_CONFIG);
        navigate(ROUTES.LOGIN);
      })
      .catch((error) => {
        setIsLoading(false);
        clearTokenData();
        navigate(ROUTES.LOGIN);
        toast.error(error.message, TOAST_CONFIG);
      });
  };

  return (
    <Dialog
      open={modalOpen}
      onClose={closeModal}
      aria-labelledby="logout-dialog-title"
      aria-describedby="logout-dialog-description"
    >
      <DialogTitle id="logout-dialog-title">Logout?</DialogTitle>
      <DialogContent>
        <DialogContentText id="logout-dialog-description">
          Once you logout, any unsaved changes will be lost.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={closeModal}>Cancel</Button>
        <Button onClick={logout}>Logout</Button>
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
