
CKEDITOR.plugins.add('cms-custompreview',
{
    init: function (editor) {
        editor.ui.addButton('cms-custompreview',
            {
                label: 'Preview',
                command: 'CustomPreview'
            });
            editor.addCommand('CustomPreview', { exec: cms.viewmodel.showpreview });
    }
});