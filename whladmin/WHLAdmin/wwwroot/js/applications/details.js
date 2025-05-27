$(function () {

  var ssnShown = false;
  $('.whl-action-togglelast4ssn').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    ssnShown = !ssnShown;
    if (ssnShown) {
      $('.whl-show-last4ssn').hide();
      $('.whl-hide-last4ssn').show();
    } else {
      $('.whl-show-last4ssn').show();
      $('.whl-hide-last4ssn').hide();
    }
  });

  var dobShown = false;
  $('.whl-action-toggledateofbirth').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    dobShown = !dobShown;
    if (dobShown) {
      $('.whl-show-dateofbirth').hide();
      $('.whl-hide-dateofbirth').show();
    } else {
      $('.whl-show-dateofbirth').show();
      $('.whl-hide-dateofbirth').hide();
    }
  });

  $('.whl-action-viewdocument').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const ai = $(this).data('applicationid');
    const di = $(this).data('docid');
    const n = $(this).data('docname');
    const a = document.createElement('a');
    a.download = '';
    a.href = documentViewUrl + '?applicationId=' + ai + '&docId=' + di;
    a.click();
  });

  $('.whl-action-deletedocument').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const ai = $(this).data('applicationid');
    const di = $(this).data('docid');
    const n = $(this).data('docname');
    if (confirm('Are you sure you want to delete the document - ' + n + '?')) {
      $.post(documentDeleteUrl + '?applicationId=' + ai + '&docId=' + di)
        .done(function (data) {
          alert('Deleted document - ' + n);
          const t = setTimeout(() => {
            clearTimeout(t);
            window.location.href = applicationDetailsUrl + '?applicationId=' + ai + '#DOCS';
            window.location.reload();
          }, 1000);
        })
        .fail(function (error) {
          console.error(error);
          alert('Failed to delete document - ' + n);
          window.location.reload();
        });
    }
  });

  $('.whl-action-withdraw-application').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#withdrawApplicationModal').modal('show');
    $('.whl-action-cancel-withdraw-application').show();
    $('.whl-action-confirm-withdraw-application').show();
    $('.whl-label-confirm-withdraw-application-inprogress').hide();
  });
  $('.whl-action-confirm-withdraw-application').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const applicationId = $(e.currentTarget).data('applicationid');
    $('.whl-action-cancel-withdraw-application').hide();
    $('.whl-action-confirm-withdraw-application').hide();
    $('.whl-label-confirm-withdraw-application-inprogress').show();
    $.post(applicationWithdrawUrl + '?applicationId=' + applicationId)
      .done(function (response) {
        $('#withdrawApplicationModal').modal('hide');
        window.location.href = applicationsUrl;
      })
      .fail(function (error) {
        $('#withdrawApplicationModal').modal('hide');
        var message = error.responseJSON && error.responseJSON.message && error.responseJSON.message.length > 0 ? error.responseJSON.message : 'Failed to withdraw application due to one or more errors';
        if (error.responseJSON && error.responseJSON.details && error.responseJSON.details.length > 0) {
          message += '<br />' + error.responseJSON.details;
        }
        alert('Failed to withdraw application due to one or more errors!');
      });
  });

  $('.whl-action-add-comment').on('click', function (e) {
    e.preventDefault();
    const applicationId = $(e.currentTarget).data('applicationid');
    $.get(commentAddUrl + '?applicationId=' + applicationId)
      .done(function (response) {
        $('#commentEditorModal').find('div.modal-body').html(response);
        $('#commentEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-comment').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    var url = commentAddUrl;
    var data = {
      ApplicationId: $('#CommentApplicationId').val(),
      CommentId: $('#CommentId').val(),
      Comments: $('#Comments').val(),
      InternalOnlyInd: $('#InternalOnlyInd').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#commentEditorModal').find('div.modal-body').html('');
        $('#commentEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Comment added.');
      })
      .fail(function (e) {
        var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save comment';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

});