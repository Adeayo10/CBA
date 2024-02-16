import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import DialogTitle from "@mui/material/DialogTitle";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import Divider from "@mui/material/Divider";

import {
  USER_ALLLOWED_FIELDS,
  USER_BRANCH_ALLOWED_FIELDS,
} from "../utils/constants";

import { capitalize } from "../utils/util";

export default function UserDetailsModal({
  toggleModal,
  modalOpen,
  user,
  userBranch,
}) {
  if (!user || !userBranch) return <></>;

  return (
    <Dialog
      open={modalOpen}
      onClose={toggleModal}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">View User</DialogTitle>

      <DialogContent>
        <Divider sx={{ mb: 1, width: "100%" }} />
        <Box sx={{ width: "100%" }}>
          <Typography gutterBottom variant="h6">
            User
          </Typography>
          <Grid
            container
            rowSpacing={1}
            columnSpacing={{ xs: 1, sm: 2, md: 3 }}
            sx={{ my: 0.5, pt: 0.5, pb: 1.5 }}
          >
            {USER_ALLLOWED_FIELDS.map((field, index) => {
              const fieldName = field.split(" ").join("");
              const fieldKey = `${user.id}_${fieldName}_${index}`;

              return (
                <Grid xs={6} key={fieldKey} item>
                  <Typography
                    gutterBottom
                    variant="subtitle2"
                    color="text.secondary"
                  >
                    {capitalize(field)}
                  </Typography>
                  <Typography variant="body" gutterBottom>
                    {capitalize(String(user[fieldName]))}
                  </Typography>
                </Grid>
              );
            })}
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
            {USER_BRANCH_ALLOWED_FIELDS.map((field, index) => {
              const fieldName = field.split(" ").join("");
              const fieldKey = `${user.id}_${fieldName}_${index}`;

              return (
                <Grid xs={6} key={fieldKey} item>
                  <Typography
                    gutterBottom
                    variant="subtitle2"
                    color="text.secondary"
                  >
                    {capitalize(field)}
                  </Typography>
                  <Typography variant="body" gutterBottom>
                    {capitalize(userBranch[fieldName])}
                  </Typography>
                </Grid>
              );
            })}
          </Grid>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={toggleModal} autoFocus>
          Done
        </Button>
      </DialogActions>
    </Dialog>
  );
}
