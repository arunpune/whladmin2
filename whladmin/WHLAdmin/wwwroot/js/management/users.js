$(function () {

  $('.whl-action-add-user').on('click', function (e) {
    e.preventDefault();
    $.get(userAddUrl)
      .done(function (response) {
        $('#whl-title-user-action').html('Add User');
        $('#hidUserAction').val('ADD');
        $('#userEditorModal').find('div.modal-body').html(response);
        $('#userEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-user').on('click', function (e) {
    e.preventDefault();
    const userId = $(e.currentTarget).data('userid');
    $.get(userEditUrl + '?userId=' + userId)
      .done(function (response) {
        $('#whl-title-user-action').html('Edit User');
        $('#hidUserAction').val('EDIT');
        $('#userEditorModal').find('div.modal-body').html(response);
        $('#userEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-user').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidUserAction').val();
    const userId = $('#UserId').val();
    const url = a === 'EDIT' ? userEditUrl : userAddUrl;
    const data = {
      UserId: $('#UserId').val(),
      EmailAddress: $('#EmailAddress').val(),
      DisplayName: $('#DisplayName').val(),
      OrganizationCd: $('#OrganizationCd').val(),
      RoleCd: $('#RoleCd').val()
    };
    $.post(url, data)
      .done(function (response) {
        $('#userEditorModal').find('div.modal-body').html('');
        $('#userEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'User ' + userId + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save user information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-user').on('click', function (e) {
    e.preventDefault();
    const userId = $(e.currentTarget).data('userid');
    if (confirm('Are you sure you want to delete user - ' + userId + '?')) {
      $.post(userDeleteUrl + '?userId=' + userId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'User ' + userId + ' deactivated, refreshing page.');
        });
    }
  });

  $('.whl-action-reactivate-user').on('click', function (e) {
    e.preventDefault();
    const userId = $(e.currentTarget).data('userid');
    if (confirm('Are you sure you want to reactivate user - ' + userId + '?')) {
      $.post(userReactivateUrl + '?userId=' + userId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'User ' + userId + ' reactivated, refreshing page.');
        });
    }
  });

});