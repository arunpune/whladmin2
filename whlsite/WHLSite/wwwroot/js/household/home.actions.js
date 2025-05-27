$(function() {

  $('.whl-action-deletemember').on('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    var i = $(e.currentTarget).data('memberid');
    var n = $(e.currentTarget).data('membername');
    var c = parseInt($(e.currentTarget).data('accountcount'));
    if (!isNaN(c) && c > 0) {
      alert('Member is associated with one or more accounts, unable to delete member!');
      return;
    }
    if (confirm('Are you sure you want to remove the following household member?\n - ' + n)) {
      $.post(deleteMemberUrl + '?memberId=' + i)
        .done(function (response) {
          window.location.reload();
        })
        .fail((error) => {
          console.log(error);
        });
    }
  });

  $('.whl-action-deleteaccount').on('click', function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();
    var i = $(e.currentTarget).data('accountid');
    var n = $(e.currentTarget).data('accountnumber');
    var t = $(e.currentTarget).data('accounttype');
    if (confirm('Are you sure you want to remove the following household account?\n - XXXX-' + n + ' (' + t + ')')) {
      $.post(deleteAccountUrl + '?accountId=' + i)
        .done(function (response) {
          window.location.reload();
        })
        .fail((error) => {
          console.log(error);
        });
    }
  });

});